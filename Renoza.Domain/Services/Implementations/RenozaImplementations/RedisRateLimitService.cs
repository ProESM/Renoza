using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Renoza.Domain.Entities.RateLimits;
using Renoza.Domain.Options;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;
using StackExchange.Redis;

namespace Renoza.Domain.Services.Implementations.RenozaImplementations
{
    /// <summary>
    /// Реализация Rate Limiting с использованием Redis и Sliding Window алгоритма
    /// </summary>
    public class RedisRateLimitService : IRateLimitService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly ILogger<RedisRateLimitService> _logger;
        private readonly RateLimitOptions _rateLimitOptions;
        private readonly RedisOptions _redisOptions;

        public RedisRateLimitService(
            IConnectionMultiplexer redis,
            ILogger<RedisRateLimitService> logger,
            IOptions<RateLimitOptions> rateLimitOptions,
            IOptions<RedisOptions> redisOptions)
        {
            _redis = redis;
            _logger = logger;
            _rateLimitOptions = rateLimitOptions.Value;
            _redisOptions = redisOptions.Value;
        }

        /// <inheritdoc/>
        public async Task<bool> IsAllowedAsync(string ipAddress, CancellationToken cancellationToken = default)
        {
            if (!_rateLimitOptions.Enabled)
            {
                return true;
            }

            // Проверка белого списка
            if (_rateLimitOptions.WhitelistedIPs.Contains(ipAddress))
            {
                _logger.LogDebug("IP {IpAddress} находится в белом списке, rate limiting пропущен", ipAddress);
                return true;
            }

            try
            {
                var db = _redis.GetDatabase();
                var now = DateTimeOffset.UtcNow;

                if (_rateLimitOptions.UseSlidingWindow)
                {
                    return await CheckSlidingWindowAsync(db, ipAddress, now);
                }
                else
                {
                    return await CheckFixedWindowAsync(db, ipAddress, now);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при проверке rate limit для IP {IpAddress}", ipAddress);
                // В случае ошибки Redis - разрешаем запрос (fail-open)
                return true;
            }
        }

        /// <inheritdoc/>
        public async Task<RateLimitInfo> GetRateLimitInfoAsync(string ipAddress, CancellationToken cancellationToken = default)
        {
            if (!_rateLimitOptions.Enabled)
            {
                return new RateLimitInfo
                {
                    CurrentCount = 0,
                    Limit = int.MaxValue,
                    WindowDuration = TimeSpan.Zero
                };
            }

            try
            {
                var db = _redis.GetDatabase();
                var now = DateTimeOffset.UtcNow;

                if (_rateLimitOptions.UseSlidingWindow)
                {
                    var key = GetSlidingWindowKey(ipAddress);
                    var windowStart = now.ToUnixTimeMilliseconds() - (_rateLimitOptions.WindowDurationSeconds * 1000);

                    // Очистка старых записей
                    await db.SortedSetRemoveRangeByScoreAsync(key, 0, windowStart);

                    var count = await db.SortedSetLengthAsync(key);

                    return new RateLimitInfo
                    {
                        CurrentCount = (int)count,
                        Limit = _rateLimitOptions.MaxRequestsPerWindow,
                        WindowDuration = TimeSpan.FromSeconds(_rateLimitOptions.WindowDurationSeconds),
                        WindowResetTime = null // Для sliding window нет фиксированного времени сброса
                    };
                }
                else
                {
                    var key = GetFixedWindowKey(ipAddress, now);
                    var countStr = await db.StringGetAsync(key);
                    var count = countStr.HasValue ? (int)countStr : 0;

                    var ttl = await db.KeyTimeToLiveAsync(key);
                    var resetTime = ttl.HasValue ? DateTime.UtcNow.Add(ttl.Value) : DateTime.UtcNow.AddSeconds(_rateLimitOptions.WindowDurationSeconds);

                    return new RateLimitInfo
                    {
                        CurrentCount = count,
                        Limit = _rateLimitOptions.MaxRequestsPerWindow,
                        WindowDuration = TimeSpan.FromSeconds(_rateLimitOptions.WindowDurationSeconds),
                        WindowResetTime = resetTime
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении информации о rate limit для IP {IpAddress}", ipAddress);
                return new RateLimitInfo
                {
                    CurrentCount = 0,
                    Limit = _rateLimitOptions.MaxRequestsPerWindow,
                    WindowDuration = TimeSpan.FromSeconds(_rateLimitOptions.WindowDurationSeconds)
                };
            }
        }

        /// <inheritdoc/>
        public async Task ResetAsync(string ipAddress, CancellationToken cancellationToken = default)
        {
            try
            {
                var db = _redis.GetDatabase();
                var now = DateTimeOffset.UtcNow;

                if (_rateLimitOptions.UseSlidingWindow)
                {
                    var key = GetSlidingWindowKey(ipAddress);
                    await db.KeyDeleteAsync(key);
                }
                else
                {
                    var key = GetFixedWindowKey(ipAddress, now);
                    await db.KeyDeleteAsync(key);
                }

                _logger.LogInformation("Счётчик rate limit сброшен для IP {IpAddress}", ipAddress);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при сбросе rate limit для IP {IpAddress}", ipAddress);
                throw;
            }
        }

        /// <summary>
        /// Проверка лимита по алгоритму Sliding Window
        /// </summary>
        private async Task<bool> CheckSlidingWindowAsync(IDatabase db, string ipAddress, DateTimeOffset now)
        {
            try
            {
                var key = GetSlidingWindowKey(ipAddress);
                var windowStart = now.ToUnixTimeMilliseconds() - (_rateLimitOptions.WindowDurationSeconds * 1000);
                var currentTimestamp = now.ToUnixTimeMilliseconds();

                // Используем транзакцию для атомарности операций
                var transaction = db.CreateTransaction();

                // Удаляем записи старше окна
                var removeTask = transaction.SortedSetRemoveRangeByScoreAsync(key, 0, windowStart);

                // Добавляем текущий запрос
                var addTask = transaction.SortedSetAddAsync(key, currentTimestamp.ToString(), currentTimestamp);

                // Устанавливаем TTL для автоматической очистки
                var expireTask = transaction.KeyExpireAsync(key, TimeSpan.FromSeconds(_rateLimitOptions.WindowDurationSeconds * 2));

                // Получаем количество запросов в окне (до добавления текущего)
                var countTask = transaction.SortedSetLengthAsync(key);

                var executed = await transaction.ExecuteAsync();

                if (!executed)
                {
                    _logger.LogWarning("Не удалось выполнить транзакцию Redis для IP {IpAddress}", ipAddress);
                    return true; // Fail-open
                }

                var count = await countTask;

                if (count > _rateLimitOptions.MaxRequestsPerWindow)
                {
                    _logger.LogWarning("Rate limit превышен для IP {IpAddress}: {Count}/{Limit}",
                        ipAddress, count, _rateLimitOptions.MaxRequestsPerWindow);

                    // Удаляем только что добавленный запрос, так как лимит превышен
                    await db.SortedSetRemoveAsync(key, currentTimestamp.ToString());
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Проверка лимита по алгоритму Fixed Window
        /// </summary>
        private async Task<bool> CheckFixedWindowAsync(IDatabase db, string ipAddress, DateTimeOffset now)
        {
            var key = GetFixedWindowKey(ipAddress, now);

            // Атомарное инкрементирование счётчика
            var count = await db.StringIncrementAsync(key);

            // Если это первый запрос в окне, устанавливаем TTL
            if (count == 1)
            {
                await db.KeyExpireAsync(key, TimeSpan.FromSeconds(_rateLimitOptions.WindowDurationSeconds));
            }

            if (count > _rateLimitOptions.MaxRequestsPerWindow)
            {
                _logger.LogWarning("Rate limit превышен для IP {IpAddress}: {Count}/{Limit}",
                    ipAddress, count, _rateLimitOptions.MaxRequestsPerWindow);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Получить ключ Redis для Sliding Window
        /// </summary>
        private string GetSlidingWindowKey(string ipAddress)
        {
            return $"{_redisOptions.KeyPrefix}ratelimit:sliding:{ipAddress}";
        }

        /// <summary>
        /// Получить ключ Redis для Fixed Window
        /// </summary>
        private string GetFixedWindowKey(string ipAddress, DateTimeOffset now)
        {
            // Округляем время до начала текущего окна
            var windowNumber = now.ToUnixTimeSeconds() / _rateLimitOptions.WindowDurationSeconds;
            return $"{_redisOptions.KeyPrefix}ratelimit:fixed:{ipAddress}:{windowNumber}";
        }
    }
}
