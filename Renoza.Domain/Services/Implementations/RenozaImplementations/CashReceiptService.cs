using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Renoza.Common.Helpers;
using Renoza.Domain.Entities.CashReceipts;
using Renoza.Domain.Entities.CashReceipts.OfdApi;
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
        /// <param name="input">Входные данные для сохранения чека</param>
        /// <param name="cancellationToken">Токен отмены</param>
        public async Task<Result<bool>> SaveCashReceiptAsync(SaveCashReceiptInput input, CancellationToken cancellationToken = default)
        {
            switch (input.InputType)
            {
                case ReceiptInputType.QrCode:
                    return await SaveCashReceiptByQrCodeAsync(input.JobId, input.Data, input.CashReceiptId, input.PdfUrl, cancellationToken);
                case ReceiptInputType.Photo:
                case ReceiptInputType.ImageFile:
                case ReceiptInputType.PdfFile:
                    return await SaveCashReceiptByFileAsync(input.JobId, input.Data, input.FileTempS3Url, input.FileName, cancellationToken);
                default:
                    throw new ArgumentOutOfRangeException(nameof(input.InputType), input.InputType, null);
            }
        }

        private async Task<Result<bool>> SaveCashReceiptByQrCodeAsync(Guid jobId, string data, Guid? cashReceiptId = null, string? pdfUrl = null, CancellationToken cancellationToken = default)
        {
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
            string? pdfS3Url = pdfUrl; // Используем переданный PDF URL (если есть)
            try
            {
                _logger.LogInformation($"JobId: {jobId}: Генерация PDF чека");

                // Если передан PDF URL от существующего чека, используем его
                if (!string.IsNullOrWhiteSpace(pdfS3Url))
                {
                    _logger.LogInformation($"JobId: {jobId}: Используется PDF ранее обработанного чека (CashReceiptId: {cashReceiptId})");
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

            // Начинаем явную транзакцию для обеспечения атомарности операций
            await BeginTransactionAsync(cancellationToken);

            try
            {
                // Если CashReceiptId не передан, создаем новый чек
                if (cashReceiptId == null)
                {
                    DateTime? documentDateTime = null;

                    if (receiptData.Document?.DateTime != null)
                    {
                        var receiptDateTime = receiptData.Document.DateTime.Value;
                        documentDateTime = new DateTime(receiptDateTime.Year,
                            receiptDateTime.Month, receiptDateTime.Day, receiptDateTime.Hour,
                            receiptDateTime.Minute, receiptDateTime.Second, DateTimeKind.Utc);
                    }

                    // Вычисляем нормализованный QR код
                    var normalizedQrSource = QrCodeNormalizer.Normalize(cashReceiptJobDao.QrSource);

                    // Сохранение данных по кассовому чеку
                    var cashReceiptDao = new CashReceiptDao
                    {
                        Id = Guid.NewGuid(),
                        QrCode = cashReceiptJobDao.QrSource,
                        NormalizedQrSource = normalizedQrSource,
                        JsonData = data,
                        FileUrl = pdfS3Url,
                        TotalAmount = receiptData.Document?.Amount_Total / 100.0m,
                        DocumentDateTime = documentDateTime,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _cashReceiptRepository.CreateAsync(cashReceiptDao, cancellationToken);

                    cashReceiptId = cashReceiptDao.Id;
                }

                // Создаем или проверяем связь клиента с чеком
                var exists = await _customerCashReceiptRepository.GetQueryable()
                    .AsNoTracking()
                    .AnyAsync(x => x.CustomerId == customerId && x.CashReceiptId == cashReceiptId.Value,
                              cancellationToken);

                if (!exists)
                {
                    var customerCashReceiptDao = new CustomerCashReceiptDao
                    {
                        CustomerId = customerId,
                        CashReceiptId = cashReceiptId.Value,
                        OrderId = cashReceiptJobDao.OrderId,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _customerCashReceiptRepository.CreateAsync(customerCashReceiptDao, cancellationToken);
                }

                // Привязываем Job к CashReceipt
                cashReceiptJobDao.CashReceiptId = cashReceiptId.Value;
                _cashReceiptJobRepository.Update(cashReceiptJobDao);

                await SaveChangesAsync(cancellationToken);

                // Коммитим транзакцию
                await CommitTransactionAsync(cancellationToken);

                _logger.LogInformation($"JobId: {jobId}: Чек успешно сохранен в транзакции (CashReceiptId: {cashReceiptId.Value})");

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                // Откатываем транзакцию при любой ошибке
                await RollbackTransactionAsync(cancellationToken);

                _logger.LogError(ex, $"JobId: {jobId}: Ошибка при сохранении чека, транзакция откачена: {ex.Message}");
                return Result<bool>.Failure(ex.Message);
            }
        }

        private async Task<Result<bool>> SaveCashReceiptByFileAsync(Guid jobId, string data, string? fileTempS3Url, string? fileName, CancellationToken cancellationToken = default)
        {
            // Получаем Job из БД для получения CustomerId
            var cashReceiptJobDao = await _cashReceiptJobRepository.GetByIdAsync(jobId, cancellationToken);
            if (cashReceiptJobDao == null)
            {
                return Result<bool>.Failure($"Не удалось получить Job с Id: {jobId}");
            }

            var customerId = cashReceiptJobDao.CustomerId;

            // Перемещаем файл из temp/ в постоянное хранилище, если есть временный URL
            string? permanentFileUrl = null;

            if (!string.IsNullOrWhiteSpace(fileTempS3Url) && !string.IsNullOrWhiteSpace(fileName))
            {
                string? tempFolderKey = null;

                try
                {
                    tempFolderKey = $"temp/receipts/{jobId}";

                    // Формируем ключ файла временного из временного хранилища
                    var tempFileKey = $"{tempFolderKey}/{fileName}";

                    // Формируем ключ для постоянного хранилища
                    var permanentFileKey = $"receipts/{customerId}/{fileName}";

                    // Формируем имя файла и путь в S3
                    var pdfFileName = $"receipt_{jobId}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.pdf";
                    var s3Folder = $"receipts/{customerId}";

                    // Перемещаем файл
                    permanentFileUrl = await _s3StorageService.MoveFileAsync(tempFileKey, permanentFileKey, cancellationToken);

                    _logger.LogInformation($"JobId: {jobId}: Файл перемещен из temp в постоянное хранилище. Новый URL: {permanentFileUrl}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"JobId: {jobId}: Ошибка при перемещении файла из temp/ в постоянное хранилище");
                    // Не прерываем процесс, продолжаем сохранение данных в БД
                }
            }

            // Десериализуем данные чека из JSON, если его нам передали

            ReceiptData? receiptData = null;

            if (!string.IsNullOrWhiteSpace(data))
            {
                receiptData = JsonConvert.DeserializeObject<ReceiptData>(data);
                if (receiptData == null)
                {
                    return Result<bool>.Failure("Не удалось десериализовать данные чека");
                }
            }

            // Начинаем явную транзакцию для обеспечения атомарности операций
            await BeginTransactionAsync(cancellationToken);

            try
            {
                DateTime? documentDateTime = receiptData?.PurchaseDateTime;

                // Сохранение данных по кассовому чеку
                var cashReceiptId = Guid.NewGuid();
                var cashReceiptDao = new CashReceiptDao
                {
                    Id = cashReceiptId,
                    QrCode = cashReceiptJobDao.QrSource,
                    NormalizedQrSource = cashReceiptId.ToString(), // так как по этому полю ищем совпадения
                    JsonData = data,
                    FileUrl = permanentFileUrl,
                    TotalAmount = receiptData?.TotalSum,
                    DocumentDateTime = documentDateTime,
                    CreatedAt = DateTime.UtcNow
                };

                await _cashReceiptRepository.CreateAsync(cashReceiptDao, cancellationToken);

                // Создаем или проверяем связь клиента с чеком
                var exists = await _customerCashReceiptRepository.GetQueryable()
                    .AsNoTracking()
                    .AnyAsync(x => x.CustomerId == customerId && x.CashReceiptId == cashReceiptId,
                              cancellationToken);

                if (!exists)
                {
                    var customerCashReceiptDao = new CustomerCashReceiptDao
                    {
                        CustomerId = customerId,
                        CashReceiptId = cashReceiptId,
                        OrderId = cashReceiptJobDao.OrderId,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _customerCashReceiptRepository.CreateAsync(customerCashReceiptDao, cancellationToken);
                }

                // Привязываем Job к CashReceipt
                cashReceiptJobDao.CashReceiptId = cashReceiptId;
                _cashReceiptJobRepository.Update(cashReceiptJobDao);

                await SaveChangesAsync(cancellationToken);

                // Коммитим транзакцию
                await CommitTransactionAsync(cancellationToken);

                _logger.LogInformation($"JobId: {jobId}: Чек успешно сохранен в транзакции (CashReceiptId: {cashReceiptId})");

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                // Откатываем транзакцию при любой ошибке
                await RollbackTransactionAsync(cancellationToken);

                _logger.LogError(ex, $"JobId: {jobId}: Ошибка при сохранении чека, транзакция откачена: {ex.Message}");
                return Result<bool>.Failure(ex.Message);
            }
        }
    }
}
