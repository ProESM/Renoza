using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Renoza.Common.Helpers;
using Renoza.Domain.Entities.CashReceipts.OfdApi;
using Renoza.Domain.Enums;
using Renoza.Domain.Services.Implementations.BaseImplementations;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;
using Renoza.Infrastructure.Contexts;
using Renoza.Infrastructure.Entities.Renoza;
using Renoza.Infrastructure.Repositories.Interfaces.BaseInterfaces;

namespace Renoza.Domain.Services.Implementations.RenozaImplementations
{
    /// <summary>
    /// Сервис для работы с кассовыми чеками
    /// </summary>
    public class CashReceiptService : BaseService<RenozaContext>, ICashReceiptService
    {
        /// <summary>
        /// Логгер
        /// </summary>
        private readonly ILogger<CashReceiptService> _logger;
        /// <summary>
        /// Сервис для генерации PDF версии кассового чека
        /// </summary>
        private readonly ICashReceiptPdfService _cashReceiptPdfService;
        /// <summary>
        /// Сервис работы с S3 хранилищем
        /// </summary>
        private readonly IS3StorageService _s3StorageService;

        #region Репозитории

        /// <summary>
        /// Репозиторий кассовых чеков
        /// </summary>
        private readonly IEntityWithIdRepository<CashReceiptDao, Guid> _cashReceiptRepository;
        /// <summary>
        /// Репозиторий связей заказчиков и кассовых чеков
        /// </summary>
        private readonly IEntityRepository<CustomerCashReceiptDao> _customerCashReceiptRepository;
        /// <summary>
        /// Репозиторий заданий на загрузку кассовых чеков
        /// </summary>
        private readonly IEntityWithIdRepository<CashReceiptJobDao, Guid> _cashReceiptJobRepository;

        #endregion

        /// <summary>
        /// Сервис для работы с кассовыми чеками
        /// </summary>
        /// <param name="logger">Логгер</param>
        /// <param name="dbContext">Контекст БД</param>
        /// <param name="cashReceiptPdfService">Сервис для генерации PDF версии кассового чека</param>
        /// <param name="s3StorageService">Сервис работы с S3 хранилищем</param>
        /// <param name="cashReceiptRepository">Репозиторий кассовых чеков</param>
        /// <param name="customerCashReceiptRepository">Репозиторий связей заказчиков и кассовых чеков</param>
        /// <param name="cashReceiptJobRepository">Репозиторий заданий на загрузку кассовых чеков</param>
        public CashReceiptService(ILogger<CashReceiptService> logger,
            RenozaContext dbContext,
            ICashReceiptPdfService cashReceiptPdfService,
            IS3StorageService s3StorageService,
            IEntityWithIdRepository<CashReceiptDao, Guid> cashReceiptRepository,
            IEntityRepository<CustomerCashReceiptDao> customerCashReceiptRepository,
            IEntityWithIdRepository<CashReceiptJobDao, Guid> cashReceiptJobRepository) : base(dbContext)
        {
            _logger = logger;
            _cashReceiptPdfService = cashReceiptPdfService;
            _s3StorageService = s3StorageService;
            _cashReceiptRepository = cashReceiptRepository;
            _customerCashReceiptRepository = customerCashReceiptRepository;
            _cashReceiptJobRepository = cashReceiptJobRepository;
        }

        /// <summary>
        /// Сохранить кассовый чек
        /// </summary>
        /// <param name="jobId">Идентификатор задания</param>
        /// <param name="inputType">Тип входных данных</param>
        /// <param name="data">Данные чека</param>
        /// <param name="contentType">MIME тип файла (опционально)</param>
        /// <param name="fileName">Имя файла (опционально)</param>
        /// <param name="existingJobId">Идентификатор ранее обработанного задания (опционально)</param>
        /// <param name="cancellationToken">Токен отмены</param>
        public async Task<Result<bool>> SaveCashReceiptAsync(Guid jobId, ReceiptInputType inputType, string data, string? contentType = null, 
            string? fileName = null, Guid? existingJobId = null, CancellationToken cancellationToken = default)
        {
            switch (inputType)
            {
                case ReceiptInputType.QrCode:
                    return await SaveCashReceiptByQrCodeAsync(jobId, data, existingJobId, cancellationToken);
                //case ReceiptInputType.Photo:
                //    break;
                //case ReceiptInputType.ImageFile:
                //    break;
                //case ReceiptInputType.PdfFile:
                //    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(inputType), inputType, null);
            }
        }

        private async Task<Result<bool>> SaveCashReceiptByQrCodeAsync(Guid jobId, string data, Guid? existingJobId = null, CancellationToken cancellationToken = default)
        {
            Guid? existingCashReceiptId = null;

            // Получаем Job из БД для получения CustomerId
            var cashReceiptJobDao = await _cashReceiptJobRepository.GetByIdAsync(jobId, cancellationToken);
            if (cashReceiptJobDao == null)
            {
                return Result<bool>.Failure($"Не удалось получить Job с Id: {jobId}");
            }

            var customerId = cashReceiptJobDao.CustomerId;

            // Десериализуем данные чека из JSON
            var receiptData = JsonConvert.DeserializeObject<OfdReceiptData>(data);
            if (receiptData == null)
            {
                return Result<bool>.Failure("Не удалось десериализовать данные чека");
            }

            // Генерируем PDF чека и сохраняем в S3
            string? pdfS3Url = null;
            try
            {
                _logger.LogInformation($"JobId: {jobId}: Генерация PDF чека");

                if (existingJobId != null)
                {
                    var existingCashReceiptJobDao = await _cashReceiptJobRepository.GetQueryable()
                        .Include(x => x.CashReceipt)
                        .FirstOrDefaultAsync(x => x.Id == existingJobId.Value, cancellationToken);
                    if (existingCashReceiptJobDao != null)
                    {
                        existingCashReceiptId = existingCashReceiptJobDao.CashReceiptId;

                        pdfS3Url = existingCashReceiptJobDao.CashReceipt?.PdfUrl;

                        if (!string.IsNullOrWhiteSpace(pdfS3Url))
                        {
                            _logger.LogInformation($"JobId: {jobId}: Взят PDF ранее обработанного чека");
                        }
                    }
                    else
                    {
                        _logger.LogError($"Не удалось получить Job с Id: {existingJobId.Value}");
                    }
                }

                if (string.IsNullOrWhiteSpace(pdfS3Url))
                {
                    // Генерируем PDF Stream
                    await using var pdfStream = await _cashReceiptPdfService.GeneratePdfAsync(receiptData);

                    // Формируем имя файла и путь в S3
                    var pdfFileName = $"receipt_{jobId}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.pdf";
                    var s3Folder = $"receipts/{customerId}";

                    // Загружаем PDF в S3
                    pdfS3Url = await _s3StorageService.UploadFileAsync(
                        pdfStream,
                        pdfFileName,
                        "application/pdf",
                        s3Folder,
                        cancellationToken);

                    _logger.LogInformation($"JobId: {jobId}: PDF чек сохранен в S3: {pdfS3Url}");
                }
            }
            catch (Exception pdfEx)
            {
                _logger.LogError(pdfEx, $"JobId: {jobId}: Ошибка при генерации или сохранении PDF чека в S3");
                // Не прерываем процесс, продолжаем сохранение данных в БД
            }

            try
            {
                if (existingCashReceiptId == null)
                {
                    DateTime? documentDateTime = null;

                    if (receiptData.Document?.DateTime != null)
                    {
                        var receiptDateTime = receiptData.Document.DateTime.Value;
                        documentDateTime = new DateTime(receiptDateTime.Year,
                            receiptDateTime.Month, receiptDateTime.Day, receiptDateTime.Hour,
                            receiptDateTime.Minute, receiptDateTime.Second, DateTimeKind.Utc);
                    }

                    // сохранение данных по кассовому чеку
                    var cashReceiptDao = new CashReceiptDao
                    {
                        Id = Guid.NewGuid(),
                        QrCode = cashReceiptJobDao.QrSource,
                        JsonData = data,
                        PdfUrl = pdfS3Url,
                        TotalAmount = receiptData.Document?.Amount_Total / 100.0m,
                        DocumentDateTime = documentDateTime,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _cashReceiptRepository.CreateAsync(cashReceiptDao, cancellationToken);

                    existingCashReceiptId = cashReceiptDao.Id;
                }

                var customerCashReceiptDao = await _customerCashReceiptRepository.GetQueryable()
                    .Where(x => x.CustomerId == customerId)
                    .Where(x => x.CashReceiptId == existingCashReceiptId.Value)
                    .FirstOrDefaultAsync(cancellationToken);

                if (customerCashReceiptDao == null)
                {
                    customerCashReceiptDao = new CustomerCashReceiptDao
                    {
                        CustomerId = customerId,
                        CashReceiptId = existingCashReceiptId.Value,
                        OrderId = null,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _customerCashReceiptRepository.CreateAsync(customerCashReceiptDao, cancellationToken);
                }

                cashReceiptJobDao.CashReceiptId = existingCashReceiptId;
                _cashReceiptJobRepository.Update(cashReceiptJobDao);

                await SaveChangesAsync(cancellationToken);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Result<bool>.Failure(ex.Message);
            }
        }
    }
}
