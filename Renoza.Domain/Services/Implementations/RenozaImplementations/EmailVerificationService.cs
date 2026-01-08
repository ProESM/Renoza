using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Renoza.Common.Base.Helpers;
using Renoza.Domain.Entities.Verifications;
using Renoza.Domain.Services.Implementations.BaseImplementations;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;
using Renoza.Infrastructure.Contexts;
using Renoza.Infrastructure.Entities.Renoza;
using Renoza.Infrastructure.Repositories.Interfaces.BaseInterfaces;

namespace Renoza.Domain.Services.Implementations.RenozaImplementations
{
    /// <summary>
    /// Сервис верификации электронной почты
    /// </summary>
    public class EmailVerificationService : BaseService<RenozaContext>, IEmailVerificationService
    {
        #region Репозитории

        /// <summary>
        /// Репозиторий верификаций email
        /// </summary>
        private readonly IEntityWithIdRepository<EmailVerificationDao, Guid> _emailVerificationRepository;

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

        #region Сервисы

        /// <summary>
        /// Сервис отправки email
        /// </summary>
        private readonly IEmailService _emailService;

        #endregion

        /// <summary>
        /// Сервис верификации электронной почты
        /// </summary>
        /// <param name="dbContext">Контекст БД</param>
        /// <param name="emailVerificationRepository">Репозиторий верификаций email</param>
        /// <param name="userRepository">Репозиторий пользователей</param>
        /// <param name="mapper">Маппер для преобразования сущностей</param>
        /// <param name="emailService">Сервис отправки email</param>
        public EmailVerificationService(
            RenozaContext dbContext,
            IEntityWithIdRepository<EmailVerificationDao, Guid> emailVerificationRepository,
            IEntityWithIdRepository<UserDao, Guid> userRepository,
            IMapper mapper,
            IEmailService emailService) : base(dbContext)
        {
            _emailVerificationRepository = emailVerificationRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _emailService = emailService;
        }

        /// <summary>
        /// Создать код верификации для email
        /// </summary>
        public async Task<Result<EmailVerification>> CreateVerificationCodeAsync(Guid userId, string email)
        {
            // Проверяем существование пользователя
            var user = await _userRepository.GetByIdAsync(userId, CancellationToken.None);
            if (user == null)
            {
                return Result<EmailVerification>.Failure($"Пользователь с Id {userId} не найден");
            }

            // Деактивируем все предыдущие коды верификации для этого email
            var existingVerifications = await _emailVerificationRepository.GetQueryable()
                .Where(v => v.UserId == userId && !v.IsVerified)
                .ToListAsync();

            foreach (var verification in existingVerifications)
            {
                verification.IsVerified = true; // Помечаем как неактуальные
                verification.UpdatedAt = DateTime.UtcNow;
                _emailVerificationRepository.Update(verification);
            }

            // Генерируем 6-значный код
            var code = GenerateVerificationCode();

            // Создаем новую запись верификации
            var emailVerification = new EmailVerificationDao
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Email = email,
                VerificationCode = code,
                IsVerified = false,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15), // Код действителен 15 минут
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _emailVerificationRepository.CreateAsync(emailVerification, CancellationToken.None);
            await SaveChangesAsync();

            // Отправляем email с кодом верификации
            var emailSent = await _emailService.SendVerificationCodeAsync(email, code);
            if (!emailSent)
            {
                // Логируем ошибку, но не блокируем процесс верификации
                // Пользователь сможет запросить код повторно
                return Result<EmailVerification>.Failure("Не удалось отправить email с кодом верификации");
            }

            return Result<EmailVerification>.Success(_mapper.Map<EmailVerification>(emailVerification));
        }

        /// <summary>
        /// Верифицировать email по коду
        /// </summary>
        public async Task<Result<bool>> VerifyEmailAsync(string email, string code)
        {
            var verification = await _emailVerificationRepository.GetQueryable()
                .Where(v => v.Email == email && v.VerificationCode == code && !v.IsVerified)
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
            _emailVerificationRepository.Update(verification);

            // Обновляем статус верификации у пользователя
            var user = await _userRepository.GetByIdAsync(verification.UserId, CancellationToken.None);
            if (user != null)
            {
                user.IsEmailVerified = true;
                user.UpdatedAt = DateTime.UtcNow;
                _userRepository.Update(user);
            }

            await SaveChangesAsync();

            return Result<bool>.Success(true);
        }

        /// <summary>
        /// Получить активную верификацию для пользователя
        /// </summary>
        public async Task<EmailVerification?> GetActiveVerificationAsync(Guid userId)
        {
            var verificationDao = await _emailVerificationRepository.GetQueryable()
                .Where(v => v.UserId == userId && !v.IsVerified && v.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(v => v.CreatedAt)
                .FirstOrDefaultAsync();

            return verificationDao != null ? _mapper.Map<EmailVerification>(verificationDao) : null;
        }

        /// <summary>
        /// Отправить код верификации повторно
        /// </summary>
        public async Task<Result<EmailVerification>> ResendVerificationCodeAsync(Guid userId, string email)
        {
            // Проверяем, не было ли недавней отправки (защита от спама)
            var recentVerification = await _emailVerificationRepository.GetQueryable()
                .Where(v => v.UserId == userId && v.CreatedAt > DateTime.UtcNow.AddMinutes(-1))
                .FirstOrDefaultAsync();

            if (recentVerification != null)
            {
                return Result<EmailVerification>.Failure("Пожалуйста, подождите перед повторной отправкой кода");
            }

            return await CreateVerificationCodeAsync(userId, email);
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
