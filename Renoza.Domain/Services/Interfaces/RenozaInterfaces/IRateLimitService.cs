using Renoza.Domain.Entities.RateLimits;

namespace Renoza.Domain.Services.Interfaces.RenozaInterfaces
{
    /// <summary>
    /// Сервис для проверки ограничения количества запросов (Rate Limiting)
    /// </summary>
    public interface IRateLimitService
    {
        /// <summary>
        /// Проверить, разрешён ли запрос с указанного IP адреса
        /// </summary>
        /// <param name="ipAddress">IP адрес клиента</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>True, если запрос разрешён; False, если достигнут лимит</returns>
        Task<bool> IsAllowedAsync(string ipAddress, CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить информацию о текущем лимите для IP адреса
        /// </summary>
        /// <param name="ipAddress">IP адрес клиента</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Информация о rate limit</returns>
        Task<RateLimitInfo> GetRateLimitInfoAsync(string ipAddress, CancellationToken cancellationToken = default);

        /// <summary>
        /// Сбросить счётчик запросов для указанного IP адреса
        /// </summary>
        /// <param name="ipAddress">IP адрес клиента</param>
        /// <param name="cancellationToken">Токен отмены</param>
        Task ResetAsync(string ipAddress, CancellationToken cancellationToken = default);
    }
}
