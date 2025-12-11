using MassTransit;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Renoza.Domain.Entities.CashReceipts.OfdApi;
using Renoza.Domain.Enums;
using Renoza.Domain.Messages.CashReceipt;
using Renoza.Domain.Options;
using Renoza.Domain.QueueConsumers.Interfaces;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;

namespace Renoza.Domain.QueueConsumers.Implementations.CashReceiptBroker
{
    /// <summary>
    /// Consumer для сохранения распознанного кассового чека в БД
    /// </summary>
    public class CashReceiptSaveConsumer : IQueueConsumer<CashReceiptSaveMessage>
    {
        private readonly ILogger<CashReceiptSaveConsumer> _logger;
        private readonly CashReceiptBrokerOptions _cashReceiptBrokerOptions;
        private readonly ICashReceiptJobService _cashReceiptJobService;
        private readonly ICashReceiptService _cashReceiptService;
        private readonly ICashReceiptPdfService _cashReceiptPdfService;
        private readonly IS3StorageService _s3StorageService;

        public CashReceiptSaveConsumer(
            ILogger<CashReceiptSaveConsumer> logger,
            CashReceiptBrokerOptions cashReceiptBrokerOptions,
            ICashReceiptJobService cashReceiptJobService,
            ICashReceiptService cashReceiptService,
            ICashReceiptPdfService cashReceiptPdfService,
            IS3StorageService s3StorageService)
        {
            _logger = logger;
            _cashReceiptBrokerOptions = cashReceiptBrokerOptions;
            _cashReceiptJobService = cashReceiptJobService;
            _cashReceiptService = cashReceiptService;
            _cashReceiptPdfService = cashReceiptPdfService;
            _s3StorageService = s3StorageService;
        }

        public async Task Consume(ConsumeContext<CashReceiptSaveMessage> context)
        {
            await ProcessMessage(context.Message);
        }

        private async Task ProcessMessage(CashReceiptSaveMessage message)
        {
            _logger.LogInformation($"Начато сохранение чека в БД. JobId: {message.JobId}");

            try
            {
                // Сохранение чека
                 var result = await _cashReceiptService.SaveCashReceiptAsync(message.JobId, ReceiptInputType.QrCode, 
                     message.ReceiptJson, existingJobId: message.ExistingJobId);

                // Временная заглушка
                _logger.LogInformation($"JobId: {message.JobId}: Чек успешно сохранен в БД.");

                // Обновляем статус на Completed
                await _cashReceiptJobService.CompleteJobAsync(message.JobId);

                _logger.LogInformation($"JobId: {message.JobId}: Обработка завершена успешно");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при сохранении чека. JobId: {message.JobId}");

                await _cashReceiptJobService.UpdateJobStatusAsync(
                    message.JobId,
                    CashReceiptJobStatus.SaveFailed,
                    $"Ошибка сохранения: {ex.Message}");

                throw;
            }
        }
    }
}
