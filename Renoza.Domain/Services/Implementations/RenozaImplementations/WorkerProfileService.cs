using AutoMapper;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
    /// Сервис работы с профилями работников
    /// </summary>
    public class WorkerProfileService : BaseService<RenozaContext>, IWorkerProfileService
    {
        #region Репозитории

        /// <summary>
        /// Репозиторий профилей работников
        /// </summary>
        private readonly IEntityWithIdRepository<WorkerProfileDao, Guid> _workerProfileRepository;

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
        private readonly ILogger<WorkerProfileService> _logger;

        #endregion

        /// <summary>
        /// Сервис работы с профилями работников
        /// </summary>
        /// <param name="dbContext">Контекст БД</param>
        /// <param name="workerProfileRepository">Репозиторий профилей работников</param>
        /// <param name="companyProfileService">Сервис для работы с профилями компаний</param>
        /// <param name="companyVerificationJobService">Сервис для работы с заданиями на верификацию</param>
        /// <param name="bus">Шина сообщений MassTransit</param>
        /// <param name="queueOptions">Настройки очередей</param>
        /// <param name="mapper">Маппер для преобразования сущностей</param>
        /// <param name="logger">Логгер</param>
        public WorkerProfileService(
            RenozaContext dbContext,
            IEntityWithIdRepository<WorkerProfileDao, Guid> workerProfileRepository,
            ICompanyProfileService companyProfileService,
            ICompanyVerificationJobService companyVerificationJobService,
            IBus bus,
            QueueOptions queueOptions,
            IMapper mapper,
            ILogger<WorkerProfileService> logger) : base(dbContext)
        {
            _workerProfileRepository = workerProfileRepository;
            _companyProfileService = companyProfileService;
            _companyVerificationJobService = companyVerificationJobService;
            _bus = bus;
            _queueOptions = queueOptions;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Возвращает интерфейс для запроса профилей работников
        /// </summary>
        /// <returns>Интерфейс для запроса профилей работников</returns>
        public IQueryable<WorkerProfile> GetQueryable()
        {
            var queryable = _workerProfileRepository.GetQueryable();
            return _mapper.ProjectTo<WorkerProfile>(queryable);
        }

        /// <summary>
        /// Получить профиль работника по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор профиля</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Профиль работника</returns>
        public async Task<WorkerProfile?> GetByIdAsync(Guid id,
            CancellationToken cancellationToken = default)
        {
            var workerProfileDao = await _workerProfileRepository.GetByIdAsync(id, cancellationToken);
            return workerProfileDao != null ? _mapper.Map<WorkerProfile>(workerProfileDao) : null;
        }

        /// <summary>
        /// Получить профиль работника по идентификатору пользователя
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Профиль работника</returns>
        public async Task<WorkerProfile?> GetByUserIdAsync(Guid userId,
            CancellationToken cancellationToken = default)
        {
            var workerProfileDao = await _workerProfileRepository.GetQueryable()
                .FirstOrDefaultAsync(p => p.UserId == userId,
                    cancellationToken);
            return workerProfileDao != null ? _mapper.Map<WorkerProfile>(workerProfileDao) : null;
        }

        /// <summary>
        /// Создать профиль работника
        /// </summary>
        /// <param name="profile">Профиль работника</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Созданный профиль</returns>
        public async Task<WorkerProfile> CreateAsync(WorkerProfile profile,
            CancellationToken cancellationToken = default)
        {
            var workerProfileDao = new WorkerProfileDao
            {
                Id = Guid.NewGuid(),
                UserId = profile.UserId,
                Specialization = profile.Specialization,
                TeamSize = profile.TeamSize,
                Certifications = profile.Certifications,
                ProfessionalStartDate = profile.ProfessionalStartDate,
                IsAvailable = true,
                Rating = profile.Rating,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _workerProfileRepository.CreateAsync(workerProfileDao, cancellationToken);
            await SaveChangesAsync(cancellationToken);
            return _mapper.Map<WorkerProfile>(workerProfileDao);
        }

        /// <summary>
        /// Создать профиль работника с созданием профиля компании и задания на верификацию
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <param name="companyInn">ИНН компании</param>
        /// <param name="ipAddress">IP адрес для создания задания на верификацию</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Созданный профиль работника</returns>
        public async Task<WorkerProfile> CreateWithCompanyAsync(Guid userId, string companyInn, string ipAddress,
            CancellationToken cancellationToken = default)
        {
            // 1. Создаем или получаем существующий профиль компании
            var companyProfileResult = await _companyProfileService.CreateCompanyProfileAsync(companyInn, cancellationToken);
            if (!companyProfileResult.IsSuccess)
            {
                throw new InvalidOperationException($"Не удалось создать профиль компании: {companyProfileResult.ErrorMessage}");
            }

            var companyProfile = companyProfileResult.Data!;

            // 2. Создаем профиль работника с привязкой к компании
            var workerProfileDao = new WorkerProfileDao
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CompanyProfileId = companyProfile.Id,
                IsAvailable = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _workerProfileRepository.CreateAsync(workerProfileDao, cancellationToken);
            await SaveChangesAsync(cancellationToken);

            // 3. Создаем задание на верификацию компании, если:
            //    - компания еще не верифицирована
            //    - нет активного задания на верификацию
            if (!companyProfile.IsCompanyVerified)
            {
                var hasActiveJob = await _companyVerificationJobService.HasActiveJobAsync(companyProfile.Id, cancellationToken);

                if (!hasActiveJob)
                {
                    var jobResult = await _companyVerificationJobService.CreateVerificationJobAsync(
                        companyProfile.Id,
                        companyInn,
                        ipAddress,
                        cancellationToken);

                    if (!jobResult.IsSuccess)
                    {
                        // Логируем ошибку, но не прерываем создание профиля
                        // Задание на верификацию можно создать позже вручную
                        _logger.LogWarning("Не удалось создать задание на верификацию для компании {CompanyInn}: {ErrorMessage}",
                            companyInn, jobResult.ErrorMessage);
                    }
                    else
                    {
                        // Отправляем сообщение в очередь на валидацию
                        var companyVerificationValidationMessage = new CompanyVerificationValidationMessage
                        {
                            JobId = jobResult.Data!.Id,
                            CompanyProfileId = companyProfile.Id,
                            Inn = companyInn,
                            IpAddress = ipAddress
                        };

                        var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{_queueOptions.CompanyVerificationValidationConsumerQueueName}"));
                        await endpoint.Send(companyVerificationValidationMessage, cancellationToken);

                        _logger.LogInformation("Задание на верификацию компании создано и отправлено в очередь. JobId: {JobId}, INN: {Inn}",
                            jobResult.Data.Id, companyInn);
                    }
                }
            }

            return _mapper.Map<WorkerProfile>(workerProfileDao);
        }

        /// <summary>
        /// Обновить профиль работника
        /// </summary>
        /// <param name="profile">Профиль работника</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Обновленный профиль</returns>
        public async Task<WorkerProfile> UpdateAsync(WorkerProfile profile,
            CancellationToken cancellationToken = default)
        {
            var workerProfileDao = await _workerProfileRepository.GetByIdAsync(profile.Id, cancellationToken);
            if (workerProfileDao == null)
                throw new InvalidOperationException($"Профиль работника с Id {profile.Id} не найден");

            workerProfileDao.Specialization = profile.Specialization;
            workerProfileDao.TeamSize = profile.TeamSize;
            workerProfileDao.Certifications = profile.Certifications;
            workerProfileDao.ProfessionalStartDate = profile.ProfessionalStartDate;
            workerProfileDao.Rating = profile.Rating;
            workerProfileDao.UpdatedAt = DateTime.UtcNow;

            _workerProfileRepository.Update(workerProfileDao);
            await SaveChangesAsync(cancellationToken);
            return _mapper.Map<WorkerProfile>(workerProfileDao);
        }

        /// <summary>
        /// Деактивировать профиль работника
        /// </summary>
        /// <param name="id">Идентификатор профиля</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Успешность операции</returns>
        public async Task<bool> DeactivateAsync(Guid id,
            CancellationToken cancellationToken = default)
        {
            var workerProfileDao = await _workerProfileRepository.GetByIdAsync(id, cancellationToken);
            if (workerProfileDao == null)
                return false;

            workerProfileDao.IsActive = false;
            workerProfileDao.UpdatedAt = DateTime.UtcNow;
            _workerProfileRepository.Update(workerProfileDao);
            await SaveChangesAsync(cancellationToken);
            return true;
        }

        /// <summary>
        /// Установить доступность работника
        /// </summary>
        /// <param name="id">Идентификатор профиля</param>
        /// <param name="isAvailable">Доступность</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Успешность операции</returns>
        public async Task<bool> SetAvailabilityAsync(Guid id, bool isAvailable,
            CancellationToken cancellationToken = default)
        {
            var workerProfileDao = await _workerProfileRepository.GetByIdAsync(id, cancellationToken);
            if (workerProfileDao == null)
                return false;

            workerProfileDao.IsAvailable = isAvailable;
            workerProfileDao.UpdatedAt = DateTime.UtcNow;
            _workerProfileRepository.Update(workerProfileDao);
            await SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
