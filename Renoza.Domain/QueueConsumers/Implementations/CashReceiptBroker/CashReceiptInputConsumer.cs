using MassTransit;
using Microsoft.Extensions.Logging;
using Renoza.Domain.Enums;
using Renoza.Domain.Exceptions;
using Renoza.Domain.Messages.CashReceipt;
using Renoza.Domain.Options;
using Renoza.Domain.QueueConsumers.Interfaces;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;

namespace Renoza.Domain.QueueConsumers.Implementations.CashReceiptBroker
{
    /// <summary>
    /// Consumer для обработки входящих кассовых чеков
    /// </summary>
    public class CashReceiptInputConsumer : IQueueConsumer<CashReceiptInputMessage>
    {
        /// <summary>
        /// Логгер
        /// </summary>
        private readonly ILogger<CashReceiptInputConsumer> _logger;
        /// <summary>
        /// Шина сообщений
        /// </summary>
        private readonly IBus _bus;
        /// <summary>
        /// Настройки брокера кассовых чеков
        /// </summary>
        private readonly CashReceiptBrokerOptions _cashReceiptBrokerOptions;
        /// <summary>
        /// Сервис для проверки Rate Limiting
        /// </summary>
        private readonly IRateLimitService _rateLimitService;
        /// <summary>
        /// Сервис для работы с заданиями на обработку чеков
        /// </summary>
        private readonly ICashReceiptJobService _cashReceiptJobService;

        /// <summary>
        /// Consumer для обработки входящих кассовых чеков
        /// </summary>
        /// <param name="logger">Логгер</param>
        /// <param name="bus">Шина сообщений</param>
        /// <param name="cashReceiptBrokerOptions">Настройки брокера кассовых чеков</param>
        /// <param name="rateLimitService">Сервис для проверки Rate Limiting</param>
        /// <param name="cashReceiptJobService">Сервис для работы с заданиями на обработку чеков</param>
        public CashReceiptInputConsumer(
            ILogger<CashReceiptInputConsumer> logger,
            IBus bus,
            CashReceiptBrokerOptions cashReceiptBrokerOptions,
            IRateLimitService rateLimitService,
            ICashReceiptJobService cashReceiptJobService)
        {
            _logger = logger;
            _bus = bus;
            _cashReceiptBrokerOptions = cashReceiptBrokerOptions;
            _rateLimitService = rateLimitService;
            _cashReceiptJobService = cashReceiptJobService;
        }

        public async Task Consume(ConsumeContext<CashReceiptInputMessage> context)
        {
            // MassTransit автоматически создаёт scope для каждого сообщения,
            // поэтому _dbContext уже является новым экземпляром для этого сообщения
            await ProcessMessage(context.Message);
        }

        private async Task ProcessMessage(CashReceiptInputMessage cashReceiptInputMessage)
        {
            _logger.LogInformation($"Получено сообщение из очереди {_cashReceiptBrokerOptions.CashReceiptInputConsumerQueueName}. IP: {cashReceiptInputMessage.IpAddress}, JobId: {cashReceiptInputMessage.JobId}");

            try
            {
                // Проверка Rate Limiting
                var isAllowed = await _rateLimitService.IsAllowedAsync(cashReceiptInputMessage.IpAddress);
                if (!isAllowed)
                {
                    _logger.LogWarning($"Сообщение отклонено из-за превышения rate limit. IP: {cashReceiptInputMessage.IpAddress}, JobId: {cashReceiptInputMessage.JobId}");

                    // Получаем информацию о лимите для логирования
                    var rateLimitInfo = await _rateLimitService.GetRateLimitInfoAsync(cashReceiptInputMessage.IpAddress);
                    _logger.LogWarning($"Rate limit для IP {cashReceiptInputMessage.IpAddress}: {rateLimitInfo.CurrentCount}/{rateLimitInfo.Limit} за {rateLimitInfo.WindowDuration.TotalSeconds}с");

                    // Обновляем статус Job на Cancelled
                    await _cashReceiptJobService.CancelJobAsync(
                        cashReceiptInputMessage.JobId,
                        $"Превышен rate limit для IP {cashReceiptInputMessage.IpAddress}");

                    // Очищаем temp хранилище (если были файлы)
                    await _cashReceiptJobService.CleanupTempStorageAsync(cashReceiptInputMessage.JobId);

                    // Отклоняем сообщение - оно попадёт в Dead Letter Queue
                    throw new RateLimitExceededException(cashReceiptInputMessage.IpAddress, rateLimitInfo.CurrentCount, rateLimitInfo.Limit);
                }

                _logger.LogInformation($"Сообщение принято для обработки. IP: {cashReceiptInputMessage.IpAddress}, JobId: {cashReceiptInputMessage.JobId}, InputType: {cashReceiptInputMessage.InputType}");

                // Роутинг по типу входных данных
                if (cashReceiptInputMessage.InputType == ReceiptInputType.QrCode)
                {
                    // QR-код: отправляем на валидацию
                    await _cashReceiptJobService.UpdateJobStatusAsync(
                        cashReceiptInputMessage.JobId,
                        CashReceiptJobStatus.Validating,
                        "Начата валидация QR кода");

                    var validationMessage = new CashReceiptValidationMessage
                    {
                        JobId = cashReceiptInputMessage.JobId,
                        InputType = cashReceiptInputMessage.InputType,
                        QrSource = cashReceiptInputMessage.Data
                    };

                    var validationEndpoint = await _bus.GetSendEndpoint(new Uri($"queue:{_cashReceiptBrokerOptions.CashReceiptValidationConsumerQueueName}"));
                    await validationEndpoint.Send(validationMessage);

                    _logger.LogInformation($"JobId: {cashReceiptInputMessage.JobId}: Сообщение обработано, отправлено на валидацию.");
                }
                else
                {
                    // Photo, ImageFile, PdfFile: пропускаем валидацию и распознавание, сразу сохраняем
                    await _cashReceiptJobService.UpdateJobStatusAsync(
                        cashReceiptInputMessage.JobId,
                        CashReceiptJobStatus.Saving,
                        "Начато сохранение введенных данных");

                    var saveMessage = new CashReceiptSaveMessage
                    {
                        JobId = cashReceiptInputMessage.JobId,
                        InputType = cashReceiptInputMessage.InputType,
                        ReceiptJson = cashReceiptInputMessage.Data, // JSON с введенными вручную данными
                        FileTempS3Url = cashReceiptInputMessage.FileTempS3Url,
                        FileName = cashReceiptInputMessage.FileName,
                        FileContentType = cashReceiptInputMessage.FileContentType
                    };

                    var saveEndpoint = await _bus.GetSendEndpoint(new Uri($"queue:{_cashReceiptBrokerOptions.CashReceiptSaveConsumerQueueName}"));
                    await saveEndpoint.Send(saveMessage);

                    _logger.LogInformation($"JobId: {cashReceiptInputMessage.JobId}: Сообщение обработано, отправлено на сохранение (минуя валидацию и распознавание).");
                }
            }
            catch (RateLimitExceededException)
            {
                // Пробрасываем исключение дальше для Dead Letter Queue
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при обработке сообщения JobId: {cashReceiptInputMessage.JobId}");

                // Обновляем статус Job на ValidationFailed
                await _cashReceiptJobService.UpdateJobStatusAsync(
                    cashReceiptInputMessage.JobId,
                    CashReceiptJobStatus.ValidationFailed,
                    $"Ошибка при обработке входного сообщения: {ex.Message}");

                // Очищаем temp хранилище (если были файлы)
                await _cashReceiptJobService.CleanupTempStorageAsync(cashReceiptInputMessage.JobId);

                throw;
            }
        }
    }
}
