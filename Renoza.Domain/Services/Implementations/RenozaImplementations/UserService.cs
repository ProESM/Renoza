using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Renoza.Common.Helpers;
using Renoza.Domain.Entities.Users;
using Renoza.Domain.Services.Implementations.BaseImplementations;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;
using Renoza.Infrastructure.Contexts;
using Renoza.Infrastructure.Entities.Renoza;
using Renoza.Infrastructure.Repositories.Interfaces.BaseInterfaces;

namespace Renoza.Domain.Services.Implementations.RenozaImplementations
{
    /// <summary>
    /// Сервис работы с пользователями
    /// </summary>
    public class UserService : BaseService<RenozaContext>, IUserService
    {
        #region Репозитории

        /// <summary>
        /// Репозиторий пользователей
        /// </summary>
        private readonly IEntityWithIdRepository<UserDao, Guid> _userRepository;

        #endregion

        #region Сервисы

        /// <summary>
        /// Сервис работы с паролями
        /// </summary>
        private readonly IPasswordService _passwordService;

        #endregion

        #region Мапперы

        /// <summary>
        /// Маппер для преобразования сущностей
        /// </summary>
        private readonly IMapper _mapper;

        #endregion

        /// <summary>
        /// Сервис работы с пользователями
        /// </summary>
        /// <param name="dbContext">Контекст БД (Scoped, новый экземпляр для каждого запроса)</param>
        /// <param name="userRepository">Репозиторий пользователей</param>
        /// <param name="passwordService">Сервис работы с паролями</param>
        /// <param name="mapper">Маппер для преобразования сущностей</param>
        public UserService(
            RenozaContext dbContext,
            IEntityWithIdRepository<UserDao, Guid> userRepository,
            IPasswordService passwordService,
            IMapper mapper) : base(dbContext)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _mapper = mapper;
        }

        /// <summary>
        /// Возвращает интерфейс для запроса пользователей
        /// </summary>
        /// <returns>Интерфейс для запроса пользователей</returns>
        public IQueryable<User> GetQueryable()
        {
            var queryable = _userRepository.GetQueryable();
            return _mapper.ProjectTo<User>(queryable);
        }

        /// <summary>
        /// Зарегистрировать нового пользователя
        /// </summary>
        public async Task<Result<User>> RegisterUserAsync(string name, string displayName, string email, string phoneNumber, string phoneCountryCode, string password)
        {
            // Проверяем уникальность имени пользователя
            var nameExists = await _userRepository.GetQueryable()
                .AsNoTracking()
                .AnyAsync(u => u.Name == name);
            if (nameExists)
            {
                return Result<User>.Failure("Пользователь с таким именем уже существует");
            }

            // Проверяем уникальность email
            var emailExists = await _userRepository.GetQueryable()
                .AsNoTracking()
                .AnyAsync(u => u.Email == email);
            if (emailExists)
            {
                return Result<User>.Failure("Пользователь с таким email уже существует");
            }

            // Проверяем уникальность телефона
            var phoneExists = await _userRepository.GetQueryable()
                .AsNoTracking()
                .AnyAsync(u => u.PhoneNumber == phoneNumber && u.PhoneCountryCode == phoneCountryCode);
            if (phoneExists)
            {
                return Result<User>.Failure("Пользователь с таким номером телефона уже существует");
            }

            // Создаем нового пользователя
            var userId = Guid.NewGuid();
            var userDao = new UserDao
            {
                Id = userId,
                Name = name,
                DisplayName = displayName,
                Email = email,
                PhoneNumber = phoneNumber,
                PhoneCountryCode = phoneCountryCode,
                IsEmailVerified = false,
                IsPhoneNumberVerified = false,
                IsActive = false, // Пользователь станет активным после верификации
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _userRepository.CreateAsync(userDao, CancellationToken.None);

            // Создаем пароль для пользователя
            var passwordCreated = await _passwordService.SetPasswordAsync(userId, password, CancellationToken.None);
            if (!passwordCreated)
            {
                return Result<User>.Failure("Не удалось создать пароль");
            }

            await SaveChangesAsync();

            return Result<User>.Success(_mapper.Map<User>(userDao));
        }
    }
}
