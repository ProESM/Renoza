using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Renoza.Common.Helpers;
using Renoza.Domain.Entities.Verifications;
using Renoza.Domain.Services.Implementations.BaseImplementations;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;
using Renoza.Infrastructure.Contexts;
using Renoza.Infrastructure.Entities.Renoza;
using Renoza.Infrastructure.Repositories.Interfaces.BaseInterfaces;

namespace Renoza.Domain.Services.Implementations.RenozaImplementations
{
    /// <summary>
    /// Сервис верификации телефонного номера
    /// </summary>
    public class PhoneVerificationService : BaseService<RenozaContext>, IPhoneVerificationService
    {
        #region Репозитории

        /// <summary>
        /// Репозиторий верификаций телефона
        /// </summary>
        private readonly IEntityWithIdRepository<PhoneVerificationDao, Guid> _phoneVerificationRepository;

        /// <summary>
        /// Репозиторий пользователей
        /// </summary>
        private readonly IEntityWithIdRepository<UserDao, Guid> _userRepository;

        #endregion

        #region Мапперы

        /// <summary>
        /// Маппер для преобразования сущностей
        /// </summary>
        private readonly IMapper _mapper;

        #endregion

        /// <summary>
        /// Сервис верификации телефонного номера
        /// </summary>
        /// <param name="dbContext">Контекст БД</param>
        /// <param name="phoneVerificationRepository">Репозиторий верификаций телефона</param>
        /// <param name="userRepository">Репозиторий пользователей</param>
        /// <param name="mapper">Маппер для преобразования сущностей</param>
        public PhoneVerificationService(
            RenozaContext dbContext,
            IEntityWithIdRepository<PhoneVerificationDao, Guid> phoneVerificationRepository,
            IEntityWithIdRepository<UserDao, Guid> userRepository,
            IMapper mapper) : base(dbContext)
        {
            _phoneVerificationRepository = phoneVerificationRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Создать код верификации для телефона
        /// </summary>
        public async Task<Result<PhoneVerification>> CreateVerificationCodeAsync(Guid userId, string phoneNumber, string phoneCountryCode)
        {
            // Проверяем существование пользователя
            var user = await _userRepository.GetByIdAsync(userId, CancellationToken.None);
            if (user == null)
            {
                return Result<PhoneVerification>.Failure($"Пользователь с Id {userId} не найден");
            }

            // Деактивируем все предыдущие коды верификации для этого телефона
            var existingVerifications = await _phoneVerificationRepository.GetQueryable()
                .Where(v => v.UserId == userId && !v.IsVerified)
                .ToListAsync();

            foreach (var verification in existingVerifications)
            {
                verification.IsVerified = true; // Помечаем как неактуальные
                verification.UpdatedAt = DateTime.UtcNow;
                _phoneVerificationRepository.Update(verification);
            }

            // Генерируем 6-значный код
            var code = GenerateVerificationCode();

            // Создаем новую запись верификации
            var phoneVerification = new PhoneVerificationDao
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                PhoneNumber = phoneNumber,
                PhoneCountryCode = phoneCountryCode,
                VerificationCode = code,
                IsVerified = false,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15), // Код действителен 15 минут
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _phoneVerificationRepository.CreateAsync(phoneVerification, CancellationToken.None);
            await SaveChangesAsync();

            // TODO: Отправить SMS с кодом верификации
            // await _smsService.SendVerificationCodeAsync(phoneCountryCode + phoneNumber, code);

            return Result<PhoneVerification>.Success(_mapper.Map<PhoneVerification>(phoneVerification));
        }

        /// <summary>
        /// Верифицировать телефон по коду
        /// </summary>
        public async Task<Result<bool>> VerifyPhoneAsync(string phoneNumber, string phoneCountryCode, string code)
        {
            var verification = await _phoneVerificationRepository.GetQueryable()
                .Where(v => v.PhoneNumber == phoneNumber
                    && v.PhoneCountryCode == phoneCountryCode
                    && v.VerificationCode == code
                    && !v.IsVerified)
                .OrderByDescending(v => v.CreatedAt)
                .FirstOrDefaultAsync();

            if (verification == null)
            {
                return Result<bool>.Failure("Неверный код верификации");
            }

            if (verification.ExpiresAt < DateTime.UtcNow)
            {
                return Result<bool>.Failure("Код верификации истек");
            }

            // Отмечаем как верифицированный
            verification.IsVerified = true;
            verification.VerifiedAt = DateTime.UtcNow;
            verification.UpdatedAt = DateTime.UtcNow;
            _phoneVerificationRepository.Update(verification);

            // Обновляем статус верификации у пользователя
            var user = await _userRepository.GetByIdAsync(verification.UserId, CancellationToken.None);
            if (user != null)
            {
                user.IsPhoneNumberVerified = true;
                user.UpdatedAt = DateTime.UtcNow;
                _userRepository.Update(user);
            }

            await SaveChangesAsync();

            return Result<bool>.Success(true);
        }

        /// <summary>
        /// Получить активную верификацию для пользователя
        /// </summary>
        public async Task<PhoneVerification?> GetActiveVerificationAsync(Guid userId)
        {
            var verificationDao = await _phoneVerificationRepository.GetQueryable()
                .Where(v => v.UserId == userId && !v.IsVerified && v.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(v => v.CreatedAt)
                .FirstOrDefaultAsync();

            return verificationDao != null ? _mapper.Map<PhoneVerification>(verificationDao) : null;
        }

        /// <summary>
        /// Отправить код верификации повторно
        /// </summary>
        public async Task<Result<PhoneVerification>> ResendVerificationCodeAsync(Guid userId, string phoneNumber, string phoneCountryCode)
        {
            // Проверяем, не было ли недавней отправки (защита от спама)
            var recentVerification = await _phoneVerificationRepository.GetQueryable()
                .Where(v => v.UserId == userId && v.CreatedAt > DateTime.UtcNow.AddMinutes(-1))
                .FirstOrDefaultAsync();

            if (recentVerification != null)
            {
                return Result<PhoneVerification>.Failure("Пожалуйста, подождите перед повторной отправкой кода");
            }

            return await CreateVerificationCodeAsync(userId, phoneNumber, phoneCountryCode);
        }

        /// <summary>
        /// Генерирует 6-значный код верификации
        /// </summary>
        private string GenerateVerificationCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
}
