using AutoMapper;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Renoza.Domain.Constants.Company;
using Renoza.Domain.Entities.Profiles;
using Renoza.Domain.Messages.CompanyVerification;
using Renoza.Domain.Options;
using Renoza.Domain.Services.Implementations.BaseImplementations;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;
using Renoza.Infrastructure.Contexts;
using Renoza.Infrastructure.Entities.Renoza;
using Renoza.Infrastructure.Repositories.Interfaces.BaseInterfaces;

namespace Renoza.Domain.Services.Implementations.RenozaImplementations
{
    /// <summary>
    /// Сервис работы с профилями технического надзора
    /// </summary>
    public class TechnicalSupervisorProfileService : BaseService<RenozaContext>, ITechnicalSupervisorProfileService
    {
        #region Репозитории

        /// <summary>
        /// Репозиторий профилей технического надзора
        /// </summary>
        private readonly IEntityWithIdRepository<TechnicalSupervisorProfileDao, Guid> _technicalSupervisorProfileRepository;

        #endregion

        #region Сервисы

        /// <summary>
        /// Сервис для работы с профилями компаний
        /// </summary>
        private readonly ICompanyProfileService _companyProfileService;

        /// <summary>
        /// Сервис для работы с заданиями на верификацию
        /// </summary>
        private readonly ICompanyVerificationJobService _companyVerificationJobService;

        /// <summary>
        /// Сервис для работы с участниками компаний
        /// </summary>
        private readonly ICompanyMemberService _companyMemberService;

        /// <summary>
        /// Сервис для работы с запросами на вступление в компанию
        /// </summary>
        private readonly ICompanyJoinRequestService _companyJoinRequestService;

        /// <summary>
        /// Шина сообщений MassTransit
        /// </summary>
        private readonly IBus _bus;

        /// <summary>
        /// Настройки очередей
        /// </summary>
        private readonly QueueOptions _queueOptions;

        #endregion

        #region Мапперы

        /// <summary>
        /// Маппер для преобразования сущностей
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Логгер
        /// </summary>
        private readonly ILogger<TechnicalSupervisorProfileService> _logger;

        #endregion

        /// <summary>
        /// Сервис работы с профилями технического надзора
        /// </summary>
        /// <param name="dbContext">Контекст БД</param>
        /// <param name="technicalSupervisorProfileRepository">Репозиторий профилей технического надзора</param>
        /// <param name="companyProfileService">Сервис для работы с профилями компаний</param>
        /// <param name="companyVerificationJobService">Сервис для работы с заданиями на верификацию</param>
        /// <param name="companyMemberService">Сервис для работы с участниками компаний</param>
        /// <param name="companyJoinRequestService">Сервис для работы с запросами на вступление в компанию</param>
        /// <param name="bus">Шина сообщений MassTransit</param>
        /// <param name="queueOptions">Настройки очередей</param>
        /// <param name="mapper">Маппер для преобразования сущностей</param>
        /// <param name="logger">Логгер</param>
        public TechnicalSupervisorProfileService(
            RenozaContext dbContext,
            IEntityWithIdRepository<TechnicalSupervisorProfileDao, Guid> technicalSupervisorProfileRepository,
            ICompanyProfileService companyProfileService,
            ICompanyVerificationJobService companyVerificationJobService,
            ICompanyMemberService companyMemberService,
            ICompanyJoinRequestService companyJoinRequestService,
            IBus bus,
            QueueOptions queueOptions,
            IMapper mapper,
            ILogger<TechnicalSupervisorProfileService> logger) : base(dbContext)
        {
            _technicalSupervisorProfileRepository = technicalSupervisorProfileRepository;
            _companyProfileService = companyProfileService;
            _companyVerificationJobService = companyVerificationJobService;
            _companyMemberService = companyMemberService;
            _companyJoinRequestService = companyJoinRequestService;
            _bus = bus;
            _queueOptions = queueOptions;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Возвращает интерфейс для запроса профилей технического надзора
        /// </summary>
        /// <returns>Интерфейс для запроса профилей</returns>
        public IQueryable<TechnicalSupervisorProfile> GetQueryable()
        {
            var queryable = _technicalSupervisorProfileRepository.GetQueryable();
            return _mapper.ProjectTo<TechnicalSupervisorProfile>(queryable);
        }

        /// <summary>
        /// Получить профиль по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор профиля</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Профиль технического надзора</returns>
        public async Task<TechnicalSupervisorProfile?> GetByIdAsync(Guid id,
            CancellationToken cancellationToken = default)
        {
            var technicalSupervisorProfileDao = await _technicalSupervisorProfileRepository.GetByIdAsync(id, cancellationToken);
            return technicalSupervisorProfileDao != null ? _mapper.Map<TechnicalSupervisorProfile>(technicalSupervisorProfileDao) : null;
        }

        /// <summary>
        /// Получить профиль по идентификатору пользователя
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Профиль технического надзора</returns>
        public async Task<TechnicalSupervisorProfile?> GetByUserIdAsync(Guid userId,
            CancellationToken cancellationToken = default)
        {
            var technicalSupervisorProfileDao = await _technicalSupervisorProfileRepository.GetQueryable()
                .FirstOrDefaultAsync(p => p.UserId == userId,
                    cancellationToken);
            return technicalSupervisorProfileDao != null ? _mapper.Map<TechnicalSupervisorProfile>(technicalSupervisorProfileDao) : null;
        }

        /// <summary>
        /// Создать профиль технического надзора
        /// </summary>
        /// <param name="profile">Профиль технического надзора</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Созданный профиль</returns>
        public async Task<TechnicalSupervisorProfile> CreateAsync(TechnicalSupervisorProfile profile,
            CancellationToken cancellationToken = default)
        {
            var technicalSupervisorProfileDao = new TechnicalSupervisorProfileDao
            {
                Id = Guid.NewGuid(),
                UserId = profile.UserId,
                Specialization = profile.Specialization,
                Certifications = profile.Certifications,
                ProfessionalStartDate = profile.ProfessionalStartDate,
                IsAvailable = true,
                Rating = profile.Rating,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _technicalSupervisorProfileRepository.CreateAsync(technicalSupervisorProfileDao, cancellationToken);
            await SaveChangesAsync(cancellationToken);
            return _mapper.Map<TechnicalSupervisorProfile>(technicalSupervisorProfileDao);
        }

        /// <summary>
        /// Создать профиль технического надзора с созданием профиля компании и задания на верификацию
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <param name="companyInn">ИНН компании</param>
        /// <param name="ipAddress">IP адрес для создания задания на верификацию</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Созданный профиль</returns>
        public async Task<TechnicalSupervisorProfile> CreateWithCompanyAsync(Guid userId, string companyInn, string ipAddress,
            CancellationToken cancellationToken = default)
        {
            // 1. Создаем или получаем существующий профиль компании
            var companyProfileResult = await _companyProfileService.CreateCompanyProfileAsync(companyInn, cancellationToken);
            if (!companyProfileResult.IsSuccess)
            {
                throw new InvalidOperationException($"Не удалось создать профиль компании: {companyProfileResult.ErrorMessage}");
            }

            var companyProfile = companyProfileResult.Data!;

            // 2. Создаем профиль технического надзора с привязкой к компании
            var technicalSupervisorProfileDao = new TechnicalSupervisorProfileDao
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                IsAvailable = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _technicalSupervisorProfileRepository.CreateAsync(technicalSupervisorProfileDao, cancellationToken);
            await SaveChangesAsync(cancellationToken);

            // 3. Проверяем, есть ли участники в компании
            var companyMembers = await _companyMemberService.GetCompanyMembersAsync(companyProfile.Id, cancellationToken);
            if (companyMembers.IsSuccess && companyMembers.Data!.Count == 0)
            {
                // TODO: Нужно подумать, как проверить, что пользователь является владельцем компании

                // Компания новая - делаем пользователя Owner'ом
                var addCompanyMemberResult = await _companyMemberService.AddMemberAsync(
                    userId,
                    companyProfile.Id,
                    CompanyMemberRoleIds.Owner,
                    cancellationToken: cancellationToken);

                if (!addCompanyMemberResult.IsSuccess)
                {
                    _logger.LogWarning("Не удалось добавить пользователя {UserId} как Owner компании {CompanyId}: {ErrorMessage}",
                        userId, companyProfile.Id, addCompanyMemberResult.ErrorMessage);
                }
            }
            else
            {
                // Компания уже существует - создаем запрос на вступление
                var companyJoinRequestResult = await _companyJoinRequestService.CreateJoinRequestAsync(
                    userId,
                    companyProfile.Id,
                    "Запрос на вступление при регистрации",
                    cancellationToken);

                if (!companyJoinRequestResult.IsSuccess)
                {
                    _logger.LogWarning("Не удалось создать запрос на вступление для пользователя {UserId} в компанию {CompanyId}: {ErrorMessage}",
                        userId, companyProfile.Id, companyJoinRequestResult.ErrorMessage);
                }
            }

            // 4. Создаем задание на верификацию компании, если:
            //    - компания еще не верифицирована
            //    - нет активного задания на верификацию
            if (!companyProfile.IsCompanyVerified)
            {
                var hasActiveJob = await _companyVerificationJobService.HasActiveJobAsync(companyProfile.Id, cancellationToken);

                if (!hasActiveJob)
                {
                    var companyVerificationJobResult = await _companyVerificationJobService.CreateVerificationJobAsync(
                        companyProfile.Id,
                        companyInn,
                        ipAddress,
                        cancellationToken);

                    if (!companyVerificationJobResult.IsSuccess)
                    {
                        // Логируем ошибку, но не прерываем создание профиля
                        // Задание на верификацию можно создать позже вручную
                        _logger.LogWarning("Не удалось создать задание на верификацию для компании {CompanyInn}: {ErrorMessage}",
                            companyInn, companyVerificationJobResult.ErrorMessage);
                    }
                    else
                    {
                        // Отправляем сообщение в очередь на валидацию
                        var companyVerificationValidationMessage = new CompanyVerificationValidationMessage
                        {
                            JobId = companyVerificationJobResult.Data!.Id,
                            CompanyProfileId = companyProfile.Id,
                            Inn = companyInn,
                            IpAddress = ipAddress
                        };

                        var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{_queueOptions.CompanyVerificationValidationConsumerQueueName}"));
                        await endpoint.Send(companyVerificationValidationMessage, cancellationToken);

                        _logger.LogInformation("Задание на верификацию компании создано и отправлено в очередь. JobId: {JobId}, INN: {Inn}",
                            companyVerificationJobResult.Data.Id, companyInn);
                    }
                }
            }

            return _mapper.Map<TechnicalSupervisorProfile>(technicalSupervisorProfileDao);
        }

        /// <summary>
        /// Обновить профиль технического надзора
        /// </summary>
        /// <param name="profile">Профиль технического надзора</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Обновленный профиль</returns>
        public async Task<TechnicalSupervisorProfile> UpdateAsync(TechnicalSupervisorProfile profile,
            CancellationToken cancellationToken = default)
        {
            var technicalSupervisorProfileDao = await _technicalSupervisorProfileRepository.GetByIdAsync(profile.Id, cancellationToken);
            if (technicalSupervisorProfileDao == null)
                throw new InvalidOperationException($"Профиль технического надзора с Id {profile.Id} не найден");

            technicalSupervisorProfileDao.Specialization = profile.Specialization;
            technicalSupervisorProfileDao.Certifications = profile.Certifications;
            technicalSupervisorProfileDao.ProfessionalStartDate = profile.ProfessionalStartDate;
            technicalSupervisorProfileDao.Rating = profile.Rating;
            technicalSupervisorProfileDao.UpdatedAt = DateTime.UtcNow;

            _technicalSupervisorProfileRepository.Update(technicalSupervisorProfileDao);
            await SaveChangesAsync(cancellationToken);
            return _mapper.Map<TechnicalSupervisorProfile>(technicalSupervisorProfileDao);
        }

        /// <summary>
        /// Деактивировать профиль технического надзора
        /// </summary>
        /// <param name="id">Идентификатор профиля</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Успешность операции</returns>
        public async Task<bool> DeactivateAsync(Guid id,
            CancellationToken cancellationToken = default)
        {
            var technicalSupervisorProfileDao = await _technicalSupervisorProfileRepository.GetByIdAsync(id, cancellationToken);
            if (technicalSupervisorProfileDao == null)
                return false;

            technicalSupervisorProfileDao.IsActive = false;
            technicalSupervisorProfileDao.UpdatedAt = DateTime.UtcNow;
            _technicalSupervisorProfileRepository.Update(technicalSupervisorProfileDao);
            await SaveChangesAsync(cancellationToken);
            return true;
        }

        /// <summary>
        /// Установить доступность для новых заказов
        /// </summary>
        /// <param name="id">Идентификатор профиля</param>
        /// <param name="isAvailable">Доступность</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Успешность операции</returns>
        public async Task<bool> SetAvailabilityAsync(Guid id, bool isAvailable,
            CancellationToken cancellationToken = default)
        {
            var technicalSupervisorProfileDao = await _technicalSupervisorProfileRepository.GetByIdAsync(id, cancellationToken);
            if (technicalSupervisorProfileDao == null)
                return false;

            technicalSupervisorProfileDao.IsAvailable = isAvailable;
            technicalSupervisorProfileDao.UpdatedAt = DateTime.UtcNow;
            _technicalSupervisorProfileRepository.Update(technicalSupervisorProfileDao);
            await SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
