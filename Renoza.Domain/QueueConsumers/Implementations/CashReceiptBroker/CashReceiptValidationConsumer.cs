using MassTransit;
using Microsoft.Extensions.Logging;
using Renoza.Domain.Entities.CashReceipts;
using Renoza.Domain.Enums;
using Renoza.Domain.Messages.CashReceipt;
using Renoza.Domain.Options;
using Renoza.Domain.QueueConsumers.Interfaces;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;
using System.Text.RegularExpressions;

namespace Renoza.Domain.QueueConsumers.Implementations.CashReceiptBroker
{
    /// <summary>
    /// Consumer для валидации QR кода кассового чека
    /// </summary>
    public class CashReceiptValidationConsumer : IQueueConsumer<CashReceiptValidationMessage>
    {
        private readonly ILogger<CashReceiptValidationConsumer> _logger;
        private readonly IBus _bus;
        private readonly CashReceiptBrokerOptions _cashReceiptBrokerOptions;
        private readonly ICashReceiptJobService _cashReceiptJobService;

        public CashReceiptValidationConsumer(
            ILogger<CashReceiptValidationConsumer> logger,
            IBus bus,
            CashReceiptBrokerOptions cashReceiptBrokerOptions,
            ICashReceiptJobService cashReceiptJobService)
        {
            _logger = logger;
            _bus = bus;
            _cashReceiptBrokerOptions = cashReceiptBrokerOptions;
            _cashReceiptJobService = cashReceiptJobService;
        }

        public async Task Consume(ConsumeContext<CashReceiptValidationMessage> context)
        {
            await ProcessMessage(context.Message);
        }

        private async Task ProcessMessage(CashReceiptValidationMessage message)
        {
            _logger.LogInformation($"Начата валидация QR кода. JobId: {message.JobId}");

            try
            {
                // Проверяем, обрабатывали ли мы ранее чек с таким же QR кодом
                var existingJobResult = await _cashReceiptJobService.FindCompletedJobByQrAsync(message.QrSource);

                if (existingJobResult.IsSuccess && existingJobResult.Data != null)
                {
                    var existingJobData = existingJobResult.Data;

                    _logger.LogInformation($"JobId: {message.JobId}: Найден ранее обработанный чек с тем же QR кодом (JobId: {existingJobData.JobId}). Пропускаем обращение к OFD API.");

                    // Обновляем статус на Recognized
                    await _cashReceiptJobService.UpdateJobStatusAsync(
                        message.JobId,
                        CashReceiptJobStatus.Recognized,
                        $"Используем данные ранее обработанного чека (JobId: {existingJobData.JobId})");

                    // Обновляем статус на Saving
                    await _cashReceiptJobService.UpdateJobStatusAsync(
                        message.JobId,
                        CashReceiptJobStatus.Saving,
                        "Сохранение распознанного чека в БД");

                    // Отправляем сообщение напрямую на сохранение, используя JSON данные из существующего чека
                    var saveMessage = new CashReceiptSaveMessage
                    {
                        JobId = message.JobId,
                        ReceiptJson = existingJobData.ReceiptJsonData,
                        ExistingJobId = existingJobData.JobId
                    };

                    var saveEndpoint = await _bus.GetSendEndpoint(new Uri($"queue:{_cashReceiptBrokerOptions.CashReceiptSaveConsumerQueueName}"));
                    await saveEndpoint.Send(saveMessage);

                    _logger.LogInformation($"JobId: {message.JobId}: Переиспользованы данные из Job {existingJobData.JobId}. Отправлено на сохранение.");

                    return;
                }

                // Валидация QR кода: извлекаем данные фискального чека
                var qrData = ParseQrCode(message.QrSource);

                if (qrData == null)
                {
                    _logger.LogWarning($"JobId: {message.JobId}: QR код имеет неверный формат");

                    await _cashReceiptJobService.UpdateJobStatusAsync(
                        message.JobId,
                        CashReceiptJobStatus.ValidationFailed,
                        "QR код имеет неверный формат");

                    return;
                }

                _logger.LogInformation($"JobId: {message.JobId}: QR код успешно распознан");

                // Обновляем статус на Queued
                await _cashReceiptJobService.UpdateJobStatusAsync(
                    message.JobId,
                    CashReceiptJobStatus.Queued,
                    "Отправка запроса в очередь на распознавание чека");

                // Отправляем сообщение в очередь распознавания
                var recognitionMessage = new CashReceiptRecognitionMessage
                {
                    JobId = message.JobId,
                    FiscalNumber = qrData.FiscalNumber,
                    FiscalDocument = qrData.FiscalDocument,
                    FiscalSign = qrData.FiscalSign,
                    ReceiptOperationType = qrData.ReceiptOperationType,
                    PurchaseDateTime = qrData.PurchaseDateTime,
                    TotalSum = qrData.TotalSum
                };

                var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{_cashReceiptBrokerOptions.CashReceiptRecognitionConsumerQueueName}"));
                await endpoint.Send(recognitionMessage);

                _logger.LogInformation($"JobId: {message.JobId}: Отправлено на распознавание");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при валидации QR кода. JobId: {message.JobId}");

                await _cashReceiptJobService.UpdateJobStatusAsync(
                    message.JobId,
                    CashReceiptJobStatus.ValidationFailed,
                    $"Ошибка валидации: {ex.Message}");

                throw;
            }
        }

        /// <summary>
        /// Парсинг QR кода кассового чека
        /// </summary>
        private QrCodeData? ParseQrCode(string qrSource)
        {
            try
            {
                // Формат QR: t=yyyyMMddTHHmm&s=сумма&fn=фн&i=фд&fp=фп&n=тип
                // Пример: t=20231215T1430&s=1500.00&fn=1234567890&i=98765&fp=1234567890&n=1

                var match = Regex.Match(qrSource,
                    @"t=(?<datetime>\d{8}T\d{4})&s=(?<sum>[\d.]+)&fn=(?<fn>\d+)&i=(?<fd>\d+)&fp=(?<fp>\d+)&n=(?<n>\d+)");

                if (!match.Success)
                    return null;

                var dateTimeStr = match.Groups["datetime"].Value;
                var purchaseDateTime = DateTime.ParseExact(dateTimeStr, "yyyyMMdd'T'HHmm", null);
                var totalSum = decimal.Parse(match.Groups["sum"].Value);

                return new QrCodeData
                {
                    FiscalNumber = match.Groups["fn"].Value,
                    FiscalDocument = match.Groups["fd"].Value,
                    FiscalSign = match.Groups["fp"].Value,
                    ReceiptOperationType = match.Groups["n"].Value,
                    PurchaseDateTime = purchaseDateTime,
                    TotalSum = totalSum
                };
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Не удалось распарсить QR код: {qrSource}");
                return null;
            }
        }
    }
}
