using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Renoza.Common.Helpers;
using Renoza.Domain.Entities.CompanyVerification;
using Renoza.Domain.Services.Implementations.BaseImplementations;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;
using Renoza.Infrastructure.Contexts;
using Renoza.Infrastructure.Entities.Renoza;
using Renoza.Infrastructure.Repositories.Interfaces.BaseInterfaces;

namespace Renoza.Domain.Services.Implementations.RenozaImplementations
{
    /// <summary>
    /// Сервис для работы с заданиями на верификацию компаний
    /// </summary>
    public class CompanyVerificationJobService : BaseService<RenozaContext>, ICompanyVerificationJobService
    {
        private readonly IEntityWithIdRepository<CompanyVerificationJobDao, Guid> _companyVerificationJobRepository;
        private readonly IEntityWithIdRepository<CompanyVerificationJobHistoryDao, long> _companyVerificationJobHistoryRepository;
        private readonly IMapper _mapper;

        public CompanyVerificationJobService(
            RenozaContext dbContext,
            IEntityWithIdRepository<CompanyVerificationJobDao, Guid> companyVerificationJobRepository,
            IEntityWithIdRepository<CompanyVerificationJobHistoryDao, long> companyVerificationJobHistoryRepository,
            IMapper mapper) : base(dbContext)
        {
            _companyVerificationJobRepository = companyVerificationJobRepository;
            _companyVerificationJobHistoryRepository = companyVerificationJobHistoryRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Создать задание на верификацию компании
        /// </summary>
        /// <param name="companyProfileId">ID профиля компании</param>
        /// <param name="inn">ИНН компании</param>
        /// <param name="ipAddress">IP адрес, с которого создано задание</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Созданное задание</returns>
        public async Task<Result<CompanyVerificationJob>> CreateVerificationJobAsync(
            Guid companyProfileId,
            string inn,
            string ipAddress,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var companyVerificationJobDao = new CompanyVerificationJobDao
                {
                    Id = Guid.NewGuid(),
                    CompanyProfileId = companyProfileId,
                    Inn = inn,
                    StatusId = (short)Enums.CompanyVerificationJobStatus.Pending,
                    IpAddress = ipAddress,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _companyVerificationJobRepository.CreateAsync(companyVerificationJobDao, cancellationToken);

                // Создаем запись в истории
                var companyVerificationJobHistoryDao = new CompanyVerificationJobHistoryDao
                {
                    JobId = companyVerificationJobDao.Id,
                    StatusId = companyVerificationJobDao.StatusId,
                    Comment = "Задание создано",
                    CreatedAt = DateTime.UtcNow
                };

                await _companyVerificationJobHistoryRepository.CreateAsync(companyVerificationJobHistoryDao, cancellationToken);
                await SaveChangesAsync(cancellationToken);

                return Result<CompanyVerificationJob>.Success(_mapper.Map<CompanyVerificationJob>(companyVerificationJobDao));
            }
            catch (Exception ex)
            {
                return Result<CompanyVerificationJob>.Failure($"Ошибка при создании задания на верификацию: {ex.Message}");
            }
        }

        /// <summary>
        /// Получить задание по ID
        /// </summary>
        /// <param name="id">ID задания</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Задание на верификацию</returns>
        public async Task<Result<CompanyVerificationJob>> GetJobByIdAsync(Guid id,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var companyVerificationJobDao = await _companyVerificationJobRepository.GetByIdAsync(id, cancellationToken);

                if (companyVerificationJobDao == null)
                {
                    return Result<CompanyVerificationJob>.Failure("Задание не найдено");
                }

                return Result<CompanyVerificationJob>.Success(_mapper.Map<CompanyVerificationJob>(companyVerificationJobDao));
            }
            catch (Exception ex)
            {
                return Result<CompanyVerificationJob>.Failure($"Ошибка при получении задания: {ex.Message}");
            }
        }

        /// <summary>
        /// Проверить, есть ли активное (незавершённое) задание на верификацию для компании
        /// </summary>
        /// <param name="companyProfileId">ID профиля компании</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>True, если есть активное задание</returns>
        public async Task<bool> HasActiveJobAsync(Guid companyProfileId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Активными считаются задания, у которых CompletedAt = null
                // (т.е. задания в статусах Pending, Validating, Queued, Processing, DataReceived, Saving)
                var hasActiveJob = await _companyVerificationJobRepository.GetQueryable()
                    .AnyAsync(j => j.CompanyProfileId == companyProfileId && j.CompletedAt == null,
                        cancellationToken);

                return hasActiveJob;
            }
            catch
            {
                // В случае ошибки возвращаем true, чтобы не создавать дубликаты
                return true;
            }
        }

        /// <summary>
        /// Проверить наличие других активных заданий на верификацию (кроме указанного)
        /// </summary>
        /// <param name="companyProfileId">ID профиля компании</param>
        /// <param name="excludeJobId">ID задания, которое нужно исключить из проверки</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>True, если есть другие активные задания</returns>
        public async Task<bool> HasOtherActiveJobAsync(Guid companyProfileId, Guid excludeJobId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Активными считаются задания, у которых CompletedAt = null И Id != excludeJobId
                var hasOtherActiveJob = await _companyVerificationJobRepository.GetQueryable()
                    .AnyAsync(j => j.CompanyProfileId == companyProfileId
                                   && j.CompletedAt == null
                                   && j.Id != excludeJobId,
                        cancellationToken);

                return hasOtherActiveJob;
            }
            catch
            {
                // В случае ошибки возвращаем true, чтобы не создавать дубликаты
                return true;
            }
        }

        /// <summary>
        /// Обновить статус задания
        /// </summary>
        /// <param name="jobId">ID задания</param>
        /// <param name="statusId">ID нового статуса</param>
        /// <param name="comment">Комментарий к изменению статуса</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат операции</returns>
        public async Task<Result<CompanyVerificationJob>> UpdateJobStatusAsync(
            Guid jobId,
            short statusId,
            string? comment = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var companyVerificationJobDao = await _companyVerificationJobRepository.GetByIdAsync(jobId, cancellationToken);

                if (companyVerificationJobDao == null)
                {
                    return Result<CompanyVerificationJob>.Failure("Задание не найдено");
                }

                companyVerificationJobDao.StatusId = statusId;
                companyVerificationJobDao.StatusComment = comment;
                companyVerificationJobDao.UpdatedAt = DateTime.UtcNow;

                _companyVerificationJobRepository.Update(companyVerificationJobDao);

                // Создаем запись в истории
                var companyVerificationJobHistoryDao = new CompanyVerificationJobHistoryDao
                {
                    JobId = jobId,
                    StatusId = statusId,
                    Comment = comment,
                    CreatedAt = DateTime.UtcNow
                };

                await _companyVerificationJobHistoryRepository.CreateAsync(companyVerificationJobHistoryDao, cancellationToken);
                await SaveChangesAsync(cancellationToken);

                return Result<CompanyVerificationJob>.Success(_mapper.Map<CompanyVerificationJob>(companyVerificationJobDao));
            }
            catch (Exception ex)
            {
                return Result<CompanyVerificationJob>.Failure($"Ошибка при обновлении статуса задания: {ex.Message}");
            }
        }

        /// <summary>
        /// Завершить задание с результатом верификации
        /// </summary>
        /// <param name="jobId">ID задания</param>
        /// <param name="companyVerificationId">ID результата верификации</param>
        /// <param name="statusId">ID финального статуса</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат операции</returns>
        public async Task<Result<CompanyVerificationJob>> CompleteJobAsync(
            Guid jobId,
            Guid companyVerificationId,
            short statusId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var companyVerificationJobDao = await _companyVerificationJobRepository.GetByIdAsync(jobId, cancellationToken);

                if (companyVerificationJobDao == null)
                {
                    return Result<CompanyVerificationJob>.Failure("Задание не найдено");
                }

                companyVerificationJobDao.StatusId = statusId;
                companyVerificationJobDao.CompanyVerificationId = companyVerificationId;
                companyVerificationJobDao.CompletedAt = DateTime.UtcNow;
                companyVerificationJobDao.UpdatedAt = DateTime.UtcNow;

                _companyVerificationJobRepository.Update(companyVerificationJobDao);

                // Создаем запись в истории
                var companyVerificationJobHistoryDao = new CompanyVerificationJobHistoryDao
                {
                    JobId = jobId,
                    StatusId = statusId,
                    Comment = "Задание завершено",
                    CreatedAt = DateTime.UtcNow
                };

                await _companyVerificationJobHistoryRepository.CreateAsync(companyVerificationJobHistoryDao, cancellationToken);
                await SaveChangesAsync(cancellationToken);

                return Result<CompanyVerificationJob>.Success(_mapper.Map<CompanyVerificationJob>(companyVerificationJobDao));
            }
            catch (Exception ex)
            {
                return Result<CompanyVerificationJob>.Failure($"Ошибка при завершении задания: {ex.Message}");
            }
        }
    }
}
