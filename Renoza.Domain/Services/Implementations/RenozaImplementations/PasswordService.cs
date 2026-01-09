using Renoza.Domain.Services.Implementations.BaseImplementations;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;
using Renoza.Infrastructure.Contexts;
using Renoza.Infrastructure.Entities.Renoza;
using Renoza.Infrastructure.Repositories.Interfaces.BaseInterfaces;
using Renoza.Domain.Options;
using Microsoft.EntityFrameworkCore;

namespace Renoza.Domain.Services.Implementations.RenozaImplementations
{
    /// <summary>
    /// Сервис работы с паролями пользователей
    /// </summary>
    public class PasswordService : BaseService<RenozaContext>, IPasswordService
    {
        #region Репозитории

        /// <summary>
        /// Репозиторий для работы с пользователями
        /// </summary>
        private readonly IEntityWithIdRepository<UserDao, Guid> _userRepository;
        /// <summary>
        /// Репозиторий для работы с паролями пользователей
        /// </summary>
        private readonly IEntityWithIdRepository<UserPasswordDao, long> _userPasswordRepository;
        /// <summary>
        /// Репозиторий для работы с историей паролей пользователей
        /// </summary>
        private readonly IEntityWithIdRepository<UserPasswordHistoryDao, long> _userPasswordHistoryRepository;

        #endregion

        #region Настройки

        /// <summary>
        /// Настройка политики паролей
        /// </summary>
        private readonly PasswordPolicyOptions _passwordPolicyOptions;

        #endregion

        /// <summary>
        /// Сервис работы с паролями пользователей
        /// </summary>
        /// <param name="context">Контекст БД (Scoped, новый экземпляр для каждого запроса)</param>
        /// <param name="passwordPolicyOptions">Настройка политики паролей</param>
        /// <param name="userRepository">Репозиторий для работы с пользователями</param>
        /// <param name="userPasswordRepository">Репозиторий для работы с паролями пользователей</param>
        /// <param name="userPasswordHistoryRepository">Репозиторий для работы с историей паролей пользователей</param>
        public PasswordService(
            RenozaContext context,
            PasswordPolicyOptions passwordPolicyOptions,
            IEntityWithIdRepository<UserDao, Guid> userRepository,
            IEntityWithIdRepository<UserPasswordDao, long> userPasswordRepository,
            IEntityWithIdRepository<UserPasswordHistoryDao, long> userPasswordHistoryRepository) : base(context)
        {
            //_dbContext = dbContext;
            _passwordPolicyOptions = passwordPolicyOptions;
            _userRepository = userRepository;
            _userPasswordRepository = userPasswordRepository;
            _userPasswordHistoryRepository = userPasswordHistoryRepository;
        }

        /// <summary>
        /// Устанавливает новый пароль для пользователя
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="password">Новый пароль в открытом виде</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>true, если пароль успешно установлен; false, если пользователь не найден или пароль уже использовался ранее</returns>
        public async Task<bool> SetPasswordAsync(Guid userId, string password, CancellationToken cancellationToken = default)
        {
            // Проверяем, что пользователь существует
            var userDao = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (userDao == null) return false;

            // Генерируем хеш пароля
            var (hash, salt) = HashPassword(password);

            // Проверяем, не использовался ли такой пароль ранее
            if (await IsPasswordInHistoryAsync(userId, hash))
            {
                return false;
            }

            // Деактивируем старые пароли
            var userPasswordDaos = await _userPasswordRepository.GetQueryable()
                .Where(p => p.UserId == userId && p.IsActive)
                .ToListAsync(cancellationToken);

            var now = DateTime.UtcNow;
            foreach (var userPasswordDao in userPasswordDaos)
            {
                userPasswordDao.IsActive = false;

                // Добавляем в историю
                var userPasswordHistoryDao = new UserPasswordHistoryDao
                {
                    UserId = userId,
                    PasswordHash = userPasswordDao.PasswordHash,
                    UsedFromAt = userPasswordDao.CreatedAt,
                    UsedToAt = now,
                    CreatedAt = now
                };
                await _userPasswordHistoryRepository.CreateAsync(userPasswordHistoryDao, cancellationToken);
            }

            // Создаем новый пароль
            var newUserPasswordDao = new UserPasswordDao
            {
                UserId = userId,
                PasswordHash = hash,
                PasswordSalt = salt,
                IsActive = true,
                CreatedAt = now,
                ExpiredAt = now.AddDays(_passwordPolicyOptions.ExpiryDays)
            };

            await _userPasswordRepository.CreateAsync(newUserPasswordDao, cancellationToken);

            // Ограничиваем размер истории паролей
            await CleanupPasswordHistory(userId, cancellationToken);

            return await SaveChangesAsync(cancellationToken) > 0;
        }

        /// <summary>
        /// Изменяет пароль пользователя с проверкой текущего пароля
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="currentPassword">Текущий пароль для проверки</param>
        /// <param name="newPassword">Новый пароль</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>true, если пароль успешно изменен; false, если текущий пароль неверный или новый пароль не соответствует требованиям</returns>
        public async Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default)
        {
            // Проверяем текущий пароль
            if (!await ValidatePasswordAsync(userId, currentPassword, cancellationToken))
            {
                return false;
            }

            // Устанавливаем новый пароль
            return await SetPasswordAsync(userId, newPassword, cancellationToken);
        }

        /// <summary>
        /// Проверяет правильность пароля пользователя
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="password">Пароль для проверки</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>true, если пароль верный и не истек; false в противном случае</returns>
        public async Task<bool> ValidatePasswordAsync(Guid userId, string password, CancellationToken cancellationToken = default)
        {
            var activeUserPasswordDao = await _userPasswordRepository.GetQueryable()
                .FirstOrDefaultAsync(p => p.UserId == userId && p.IsActive, cancellationToken);

            if (activeUserPasswordDao == null) return false;

            // Проверяем срок действия пароля
            if (activeUserPasswordDao.ExpiredAt.HasValue && activeUserPasswordDao.ExpiredAt < DateTime.UtcNow)
            {
                return false;
            }

            // Проверяем хеш пароля
            return VerifyPassword(password, activeUserPasswordDao.PasswordHash, activeUserPasswordDao.PasswordSalt);
        }

        /// <summary>
        /// Проверяет, использовался ли пароль ранее в истории паролей пользователя
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="newPasswordHash">Хеш нового пароля для проверки</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>true, если пароль уже использовался ранее; false в противном случае</returns>
        public async Task<bool> IsPasswordInHistoryAsync(Guid userId, string newPasswordHash, CancellationToken cancellationToken = default)
        {
            var recentPasswords = await _userPasswordHistoryRepository.GetQueryable()
                .Where(ph => ph.UserId == userId)
                .OrderByDescending(ph => ph.UsedFromAt)
                .Take(_passwordPolicyOptions.HistorySize)
                .Select(ph => ph.PasswordHash)
                .ToListAsync(cancellationToken);

            return recentPasswords.Contains(newPasswordHash);
        }

        /// <summary>
        /// Проверяет, истек ли срок действия пароля пользователя
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>true, если срок действия пароля истек; false в противном случае</returns>
        public async Task<bool> IsPasswordExpiredAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var activeUserPasswordDao = await _userPasswordRepository.GetQueryable()
                .FirstOrDefaultAsync(p => p.UserId == userId && p.IsActive, cancellationToken);

            return activeUserPasswordDao?.ExpiredAt < DateTime.UtcNow;
        }

        /// <summary>
        /// Очищает историю паролей пользователя, удаляя самые старые записи, если их количество превышает заданный лимит
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        private async Task CleanupPasswordHistory(Guid userId, CancellationToken cancellationToken = default)
        {
            var historyCount = await _userPasswordHistoryRepository.GetQueryable()
                .CountAsync(ph => ph.UserId == userId, cancellationToken);

            if (historyCount > _passwordPolicyOptions.HistorySize)
            {
                var oldUserPasswordHistoryDaos = await _userPasswordHistoryRepository.GetQueryable()
                    .Where(ph => ph.UserId == userId)
                    .OrderBy(ph => ph.UsedFromAt)
                    .Take(historyCount - _passwordPolicyOptions.HistorySize)
                    .ToListAsync(cancellationToken);

                _userPasswordHistoryRepository.DeleteRange(oldUserPasswordHistoryDaos);
            }
        }

        /// <summary>
        /// Генерирует хеш и соль для пароля с использованием алгоритма HMACSHA512
        /// </summary>
        /// <param name="password">Пароль в открытом виде</param>
        /// <returns>Кортеж, содержащий хеш пароля и соль в формате Base64</returns>
        private (string hash, string salt) HashPassword(string password)
        {
            using var hmac = new System.Security.Cryptography.HMACSHA512();
            var salt = Convert.ToBase64String(hmac.Key);
            var hash = Convert.ToBase64String(hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)));
            return (hash, salt);
        }

        /// <summary>
        /// Проверяет соответствие пароля сохраненному хешу с использованием сохраненной соли
        /// </summary>
        /// <param name="password">Пароль для проверки в открытом виде</param>
        /// <param name="storedHash">Сохраненный хеш пароля в формате Base64</param>
        /// <param name="storedSalt">Сохраненная соль в формате Base64</param>
        /// <returns>true, если пароль соответствует сохраненному хешу; false в противном случае</returns>
        private bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            var saltBytes = Convert.FromBase64String(storedSalt);
            using var hmac = new System.Security.Cryptography.HMACSHA512(saltBytes);
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(computedHash) == storedHash;
        }
    }
}
