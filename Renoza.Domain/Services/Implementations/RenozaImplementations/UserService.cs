using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Renoza.Common.Helpers;
using Renoza.Domain.Entities.Auth;
using Renoza.Domain.Entities.Profiles;
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

        /// <summary>
        /// Сервис работы с профилями заказчиков
        /// </summary>
        private readonly ICustomerProfileService _customerProfileService;

        /// <summary>
        /// Сервис работы с профилями работников
        /// </summary>
        private readonly IWorkerProfileService _workerProfileService;

        /// <summary>
        /// Сервис работы с профилями технических руководителей
        /// </summary>
        private readonly ITechnicalSupervisorProfileService _technicalSupervisorProfileService;

        /// <summary>
        /// Сервис работы с ролями
        /// </summary>
        private readonly IRoleService _roleService;

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
        /// <param name="customerProfileService">Сервис работы с профилями заказчиков</param>
        /// <param name="workerProfileService">Сервис работы с профилями работников</param>
        /// <param name="technicalSupervisorProfileService">Сервис работы с профилями технических руководителей</param>
        /// <param name="roleService">Сервис работы с ролями</param>
        /// <param name="mapper">Маппер для преобразования сущностей</param>
        public UserService(
            RenozaContext dbContext,
            IEntityWithIdRepository<UserDao, Guid> userRepository,
            IPasswordService passwordService,
            ICustomerProfileService customerProfileService,
            IWorkerProfileService workerProfileService,
            ITechnicalSupervisorProfileService technicalSupervisorProfileService,
            IRoleService roleService,
            IMapper mapper) : base(dbContext)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _customerProfileService = customerProfileService;
            _workerProfileService = workerProfileService;
            _technicalSupervisorProfileService = technicalSupervisorProfileService;
            _roleService = roleService;
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
        public async Task<Result<User>> RegisterUserAsync(RegisterUserInput input, CancellationToken cancellationToken = default)
        {
            // Проверяем уникальность имени пользователя
            var nameExists = await _userRepository.GetQueryable()
                .AsNoTracking()
                .AnyAsync(u => u.Name == input.Name, cancellationToken);
            if (nameExists)
            {
                return Result<User>.Failure("Пользователь с таким именем уже существует");
            }

            // Проверяем уникальность email
            var emailExists = await _userRepository.GetQueryable()
                .AsNoTracking()
                .AnyAsync(u => u.Email == input.Email, cancellationToken);
            if (emailExists)
            {
                return Result<User>.Failure("Пользователь с таким email уже существует");
            }

            // Проверяем уникальность телефона
            var phoneExists = await _userRepository.GetQueryable()
                .AsNoTracking()
                .AnyAsync(u => u.PhoneNumber == input.PhoneNumber && u.PhoneCountryCode == input.PhoneCountryCode, cancellationToken);
            if (phoneExists)
            {
                return Result<User>.Failure("Пользователь с таким номером телефона уже существует");
            }

            // Начинаем явную транзакцию для атомарности создания User + Password + Profile
            await BeginTransactionAsync(cancellationToken);

            try
            {
                // Создаем нового пользователя
                var userId = Guid.NewGuid();
                var userDao = new UserDao
                {
                    Id = userId,
                    Name = input.Name,
                    DisplayName = input.DisplayName,
                    Email = input.Email,
                    PhoneNumber = input.PhoneNumber,
                    PhoneCountryCode = input.PhoneCountryCode,
                    IsEmailVerified = false,
                    IsPhoneNumberVerified = false,
                    IsActive = false, // Пользователь станет активным после верификации
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _userRepository.CreateAsync(userDao, cancellationToken);

                // Создаем пароль для пользователя
                var passwordCreated = await _passwordService.SetPasswordAsync(userId, input.Password, cancellationToken);
                if (!passwordCreated)
                {
                    await RollbackTransactionAsync(cancellationToken);
                    return Result<User>.Failure("Не удалось создать пароль");
                }

                // Создаем профиль в зависимости от роли
                var userRole = input.UserRole?.Trim();

                if (userRole == "Customer")
                {
                    // Создаем профиль заказчика
                    var customerProfile = new CustomerProfile
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId
                    };

                    await _customerProfileService.CreateAsync(customerProfile);
                }
                else if (userRole == "Worker")
                {
                    // Для работника требуется ИНН компании
                    if (string.IsNullOrWhiteSpace(input.CompanyInn))
                    {
                        await RollbackTransactionAsync(cancellationToken);
                        return Result<User>.Failure("Для роли Worker необходимо указать ИНН компании");
                    }

                    await _workerProfileService.CreateWithCompanyAsync(
                        userId,
                        input.CompanyInn,
                        input.IpAddress ?? string.Empty,
                        cancellationToken);
                }
                else if (userRole == "TechnicalSupervisor")
                {
                    // Для технического руководителя требуется ИНН компании
                    if (string.IsNullOrWhiteSpace(input.CompanyInn))
                    {
                        await RollbackTransactionAsync(cancellationToken);
                        return Result<User>.Failure("Для роли TechnicalSupervisor необходимо указать ИНН компании");
                    }

                    await _technicalSupervisorProfileService.CreateWithCompanyAsync(
                        userId,
                        input.CompanyInn,
                        input.IpAddress ?? string.Empty,
                        cancellationToken);
                }
                else
                {
                    await RollbackTransactionAsync(cancellationToken);
                    return Result<User>.Failure($"Неизвестная роль пользователя: {userRole}");
                }

                // Назначаем роль пользователю
                if (!string.IsNullOrWhiteSpace(userRole))
                {
                    var roleId = _roleService.GetRoleIdByName(userRole);
                    if (roleId.HasValue)
                    {
                        var roleAssigned = await _roleService.AssignRoleToUserAsync(userId, roleId.Value);
                        if (!roleAssigned)
                        {
                            await RollbackTransactionAsync(cancellationToken);
                            return Result<User>.Failure($"Не удалось назначить роль {userRole} пользователю");
                        }
                    }
                    else
                    {
                        await RollbackTransactionAsync(cancellationToken);
                        return Result<User>.Failure($"Роль {userRole} не найдена в системе");
                    }
                }

                await SaveChangesAsync(cancellationToken);

                // Коммитим транзакцию
                await CommitTransactionAsync(cancellationToken);

                return Result<User>.Success(_mapper.Map<User>(userDao));
            }
            catch (Exception ex)
            {
                // Откатываем транзакцию при любой ошибке
                await RollbackTransactionAsync(cancellationToken);
                return Result<User>.Failure($"Ошибка при регистрации пользователя: {ex.Message}");
            }
        }
    }
}
