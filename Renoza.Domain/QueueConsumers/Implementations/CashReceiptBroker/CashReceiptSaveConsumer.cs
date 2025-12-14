using MassTransit;
using Microsoft.Extensions.Logging;
using Renoza.Domain.Entities.CashReceipts;
using Renoza.Domain.Enums;
using Renoza.Domain.Messages.CashReceipt;
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
        private readonly ICashReceiptJobService _cashReceiptJobService;
        private readonly ICashReceiptService _cashReceiptService;
        private readonly IS3StorageService _s3StorageService;

        public CashReceiptSaveConsumer(
            ILogger<CashReceiptSaveConsumer> logger,
            ICashReceiptJobService cashReceiptJobService,
            ICashReceiptService cashReceiptService,
            IS3StorageService s3StorageService)
        {
            _logger = logger;
            _cashReceiptJobService = cashReceiptJobService;
            _cashReceiptService = cashReceiptService;
            _s3StorageService = s3StorageService;
        }

        public async Task Consume(ConsumeContext<CashReceiptSaveMessage> context)
        {
            await ProcessMessage(context.Message);
        }

        private async Task ProcessMessage(CashReceiptSaveMessage message)
        {
            _logger.LogInformation($"Начато сохранение чека в БД. JobId: {message.JobId}");

            string? tempFolderKey = null;

            try
            {
                // Сохранение чека
                var input = new SaveCashReceiptInput
                {
                    JobId = message.JobId,
                    InputType = message.InputType,
                    Data = message.ReceiptJson,
                    CashReceiptId = message.CashReceiptId,
                    FileTempS3Url = message.FileTempS3Url,
                    ContentType = message.FileContentType,
                    FileName = message.FileName
                };

                var result = await _cashReceiptService.SaveCashReceiptAsync(input);

                if (!result.IsSuccess)
                {
                    _logger.LogError($"JobId: {message.JobId}: Ошибка при сохранении чека: {result.ErrorMessage}");

                    await _cashReceiptJobService.UpdateJobStatusAsync(
                        message.JobId,
                        CashReceiptJobStatus.SaveFailed,
                        $"Ошибка сохранения: {result.ErrorMessage}");

                    // Удаляем временную папку при ошибке
                    if (!string.IsNullOrWhiteSpace(tempFolderKey))
                    {
                        await CleanupTempFolder(tempFolderKey, message.JobId);
                    }

                    return;
                }

                _logger.LogInformation($"JobId: {message.JobId}: Чек успешно сохранен в БД.");

                // Удаляем временную папку после успешного сохранения
                if (!string.IsNullOrWhiteSpace(tempFolderKey))
                {
                    await CleanupTempFolder(tempFolderKey, message.JobId);
                }

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

                // Удаляем временную папку при ошибке
                if (!string.IsNullOrWhiteSpace(tempFolderKey))
                {
                    await CleanupTempFolder(tempFolderKey, message.JobId);
                }

                throw;
            }
        }

        /// <summary>
        /// Удаляет временную папку в S3
        /// </summary>
        private async Task CleanupTempFolder(string tempFolderKey, Guid jobId)
        {
            try
            {
                await _s3StorageService.DeleteFolderAsync(tempFolderKey);
                _logger.LogInformation($"JobId: {jobId}: Временная папка {tempFolderKey} успешно удалена");
            }
            catch (Exception ex)
            {
                // Логируем ошибку, но не пробрасываем её дальше
                _logger.LogWarning(ex, $"JobId: {jobId}: Не удалось удалить временную папку {tempFolderKey}. Файлы будут удалены по расписанию через Lifecycle Policy");
            }
        }
    }
}
