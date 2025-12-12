using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Renoza.Common.Helpers;
using Renoza.Domain.Entities.CashReceipts;
using Renoza.Domain.Enums;
using Renoza.Domain.Helpers;
using Renoza.Domain.Services.Implementations.BaseImplementations;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;
using Renoza.Infrastructure.Contexts;
using Renoza.Infrastructure.Entities.Renoza;
using Renoza.Infrastructure.Repositories.Interfaces.BaseInterfaces;

namespace Renoza.Domain.Services.Implementations.RenozaImplementations
{
    /// <summary>
    /// Сервис для управления запросами на загрузку чеков (Job)
    /// </summary>
    public class CashReceiptJobService : BaseService<RenozaContext>, ICashReceiptJobService
    {
        private readonly ILogger<CashReceiptJobService> _logger;
        private readonly IMapper _mapper;

        #region Репозитории

        private readonly IEntityWithIdRepository<CashReceiptDao, Guid> _cashReceiptRepository;
        private readonly IEntityWithIdRepository<CashReceiptJobDao, Guid> _cashReceiptJobRepository;
        private readonly IEntityWithIdRepository<CashReceiptJobHistoryDao, long> _cashReceiptJobHistoryRepository;

        #endregion

        public CashReceiptJobService(
            ILogger<CashReceiptJobService> logger,
            IMapper mapper,
            RenozaContext dbContext,
            IEntityWithIdRepository<CashReceiptDao, Guid> cashReceiptRepository,
            IEntityWithIdRepository<CashReceiptJobDao, Guid> cashReceiptJobRepository,
            IEntityWithIdRepository<CashReceiptJobHistoryDao, long> cashReceiptJobHistoryRepository)
            : base(dbContext)
        {
            _logger = logger;
            _mapper = mapper;
            _cashReceiptRepository = cashReceiptRepository;
            _cashReceiptJobRepository = cashReceiptJobRepository;
            _cashReceiptJobHistoryRepository = cashReceiptJobHistoryRepository;
        }

        /// <summary>
        /// Создать новый запрос на загрузку чека
        /// </summary>
        public async Task<Result<Guid>> CreateJobAsync(
            CreateCashReceiptJobInput input,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Создаем новый Job с начальным статусом Pending
                var jobDao = new CashReceiptJobDao
                {
                    Id = Guid.NewGuid(),
                    CustomerId = input.CustomerId,
                    CreatedBy = input.CreatedBy,
                    OrderId = input.OrderId,
                    StatusId = (short)CashReceiptJobStatus.Pending,
                    QrSource = input.QrSource,
                    IpAddress = input.IpAddress,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _cashReceiptJobRepository.CreateAsync(jobDao, cancellationToken);

                // Создаем запись в истории
                var historyDao = new CashReceiptJobHistoryDao
                {
                    JobId = jobDao.Id,
                    StatusId = (short)CashReceiptJobStatus.Pending,
                    Comment = "Запрос создан",
                    CreatedAt = DateTime.UtcNow
                };

                await _cashReceiptJobHistoryRepository.CreateAsync(historyDao, cancellationToken);
                await SaveChangesAsync(cancellationToken);

                _logger.LogInformation($"Создан запрос на загрузку чека: {jobDao.Id}");

                return Result<Guid>.Success(jobDao.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при создании запроса на загрузку чека: {ex.Message}");
                return Result<Guid>.Failure($"Ошибка при создании запроса: {ex.Message}");
            }
        }

        /// <summary>
        /// Обновить статус запроса
        /// </summary>
        public async Task<Result<bool>> UpdateJobStatusAsync(
            Guid jobId,
            CashReceiptJobStatus newStatus,
            string? comment = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var job = await _cashReceiptJobRepository.GetQueryable()
                    .FirstOrDefaultAsync(x => x.Id == jobId, cancellationToken);

                if (job == null)
                {
                    return Result<bool>.Failure($"Запрос с ID {jobId} не найден");
                }

                // Обновляем статус
                job.StatusId = (short)newStatus;
                job.StatusComment = comment;
                job.UpdatedAt = DateTime.UtcNow;

                // Если статус терминальный, устанавливаем дату завершения
                var isTerminalStatus = newStatus switch
                {
                    CashReceiptJobStatus.Completed => true,
                    CashReceiptJobStatus.Cancelled => true,
                    CashReceiptJobStatus.ValidationFailed => true,
                    CashReceiptJobStatus.RecognitionFailed => true,
                    CashReceiptJobStatus.SaveFailed => true,
                    _ => false
                };

                if (isTerminalStatus)
                {
                    job.CompletedAt = DateTime.UtcNow;
                }

                _cashReceiptJobRepository.Update(job);

                // Добавляем запись в историю
                var historyDao = new CashReceiptJobHistoryDao
                {
                    JobId = jobId,
                    StatusId = (short)newStatus,
                    Comment = comment,
                    CreatedAt = DateTime.UtcNow
                };

                await _cashReceiptJobHistoryRepository.CreateAsync(historyDao, cancellationToken);
                await SaveChangesAsync(cancellationToken);

                _logger.LogInformation($"Обновлен статус запроса {jobId} на {newStatus}");

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при обновлении статуса запроса {jobId}: {ex.Message}");
                return Result<bool>.Failure($"Ошибка при обновлении статуса: {ex.Message}");
            }
        }

        /// <summary>
        /// Получить запрос по идентификатору
        /// </summary>
        public async Task<Result<CashReceiptJob>> GetJobByIdAsync(
            Guid jobId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var jobDao = await _cashReceiptJobRepository.GetQueryable()
                    .AsNoTracking()
                    .Include(x => x.Status)
                    .FirstOrDefaultAsync(x => x.Id == jobId, cancellationToken);

                if (jobDao == null)
                {
                    return Result<CashReceiptJob>.Failure($"Запрос с ID {jobId} не найден");
                }

                var job = _mapper.Map<CashReceiptJob>(jobDao);
                return Result<CashReceiptJob>.Success(job);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при получении запроса {jobId}: {ex.Message}");
                return Result<CashReceiptJob>.Failure($"Ошибка при получении запроса: {ex.Message}");
            }
        }

        /// <summary>
        /// Получить историю изменений статусов запроса
        /// </summary>
        public async Task<Result<List<CashReceiptJobHistory>>> GetJobHistoryAsync(
            Guid jobId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var historyDaoList = await _cashReceiptJobHistoryRepository.GetQueryable()
                    .AsNoTracking()
                    .Include(x => x.Status)
                    .Where(x => x.JobId == jobId)
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync(cancellationToken);

                var history = _mapper.Map<List<CashReceiptJobHistory>>(historyDaoList);
                return Result<List<CashReceiptJobHistory>>.Success(history);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при получении истории запроса {jobId}: {ex.Message}");
                return Result<List<CashReceiptJobHistory>>.Failure($"Ошибка при получении истории: {ex.Message}");
            }
        }

        /// <summary>
        /// Отметить запрос как завершенный
        /// </summary>
        public async Task<Result<bool>> CompleteJobAsync(
            Guid jobId,
            CancellationToken cancellationToken = default)
        {
            return await UpdateJobStatusAsync(jobId, CashReceiptJobStatus.Completed, "Обработка завершена успешно", cancellationToken);
        }

        /// <summary>
        /// Отменить запрос
        /// </summary>
        public async Task<Result<bool>> CancelJobAsync(
            Guid jobId,
            string? comment = null,
            CancellationToken cancellationToken = default)
        {
            return await UpdateJobStatusAsync(jobId, CashReceiptJobStatus.Cancelled, comment ?? "Запрос отменен", cancellationToken);
        }

        /// <summary>
        /// Найти успешно обработанный чек по QR коду
        /// </summary>
        public async Task<Result<CompletedCashReceiptJobResult?>> FindCompletedJobByQrAsync(
            string qrSource,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var normalizedQr = QrCodeNormalizer.Normalize(qrSource);

                // Ищем чек напрямую в таблице CashReceipts по нормализованному QR коду
                var receiptDao = await _cashReceiptRepository.GetQueryable()
                    .AsNoTracking()
                    .Where(x => x.NormalizedQrSource == normalizedQr)
                    .FirstOrDefaultAsync(cancellationToken);

                if (receiptDao == null)
                {
                    return Result<CompletedCashReceiptJobResult?>.Success(null);
                }

                // Возвращаем CashReceiptId, JSON данные чека и PDF URL
                var result = new CompletedCashReceiptJobResult
                {
                    CashReceiptId = receiptDao.Id,
                    ReceiptJsonData = receiptDao.JsonData ?? string.Empty,
                    PdfUrl = receiptDao.PdfUrl
                };

                return Result<CompletedCashReceiptJobResult?>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при поиске обработанного чека по QR коду: {ex.Message}");
                return Result<CompletedCashReceiptJobResult?>.Failure($"Ошибка при поиске чека: {ex.Message}");
            }
        }
    }
}
