using MassTransit;
using Microsoft.Extensions.Logging;
using Renoza.Domain.Entities.CashReceipts;
using Renoza.Domain.Enums;
using Renoza.Domain.Exceptions;
using Renoza.Domain.Messages.CashReceipt;
using Renoza.Domain.Options;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;

namespace Renoza.Domain.Services.Implementations.RenozaImplementations
{
    /// <summary>
    /// Сервис для работы с чеками
    /// </summary>
    public class ReceiptService : IReceiptService
    {
        /// <summary>
        /// Шина сообщений
        /// </summary>
        private readonly IBus _bus;
        /// <summary>
        /// Логгер
        /// </summary>
        private readonly ILogger<ReceiptService> _logger;
        /// <summary>
        /// Настройки очередей
        /// </summary>
        private readonly QueueOptions _queueOptions;
        /// <summary>
        /// Сервис для проверки Rate Limiting
        /// </summary>
        private readonly IRateLimitService _rateLimitService;
        /// <summary>
        /// Сервис для работы с заданиями на обработку чеков
        /// </summary>
        private readonly ICashReceiptJobService _cashReceiptJobService;

        /// <summary>
        /// Сервис для работы с чеками
        /// </summary>
        /// <param name="bus">Шина сообщений</param>
        /// <param name="logger">Логгер</param>
        /// <param name="queueOptions">Настройки очередей</param>
        /// <param name="rateLimitService">Сервис для проверки Rate Limiting</param>
        /// <param name="cashReceiptJobService">Сервис для работы с заданиями на обработку чеков</param>
        public ReceiptService(IBus bus,
            ILogger<ReceiptService> logger,
            QueueOptions queueOptions,
            IRateLimitService rateLimitService,
            ICashReceiptJobService cashReceiptJobService)
        {
            _bus = bus;
            _logger = logger;
            _queueOptions = queueOptions;
            _rateLimitService = rateLimitService;
            _cashReceiptJobService = cashReceiptJobService;
        }

        /// <summary>
        /// Загрузить кассовый чек (отправить в очередь обработки)
        /// </summary>
        /// <param name="input">Входные данные для загрузки чека</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Идентификатор задания на обработку</returns>
        public async Task<Guid> UploadCashReceiptAsync(
            UploadCashReceiptInput input,
            CancellationToken cancellationToken = default)
        {
            // Проверка Rate Limiting
            var isAllowed = await _rateLimitService.IsAllowedAsync(input.IpAddress, cancellationToken);
            if (!isAllowed)
            {
                _logger.LogWarning($"Запрос отклонён из-за превышения rate limit. IP: {input.IpAddress}");

                // Получаем информацию о лимите для логирования
                var rateLimitInfo = await _rateLimitService.GetRateLimitInfoAsync(input.IpAddress);
                _logger.LogWarning($"Rate limit для IP {input.IpAddress}: {rateLimitInfo.CurrentCount}/{rateLimitInfo.Limit} за {rateLimitInfo.WindowDuration.TotalSeconds}с");

                // Отклоняем запрос
                throw new RateLimitExceededException(input.IpAddress, rateLimitInfo.CurrentCount, rateLimitInfo.Limit);
            }

            // Создаём Job в БД
            var createJobInput = new CreateCashReceiptJobInput
            {
                CustomerId = input.Metadata.CustomerId,
                CreatedBy = input.CreatedBy,
                QrSource = input.Data,
                IpAddress = input.IpAddress,
                OrderId = input.Metadata.OrderId
            };

            var jobResult = await _cashReceiptJobService.CreateJobAsync(createJobInput, cancellationToken);
            if (!jobResult.IsSuccess)
            {
                _logger.LogError($"Ошибка при создании задания на обработку чека: {jobResult.ErrorMessage}");
                throw new Exception($"Не удалось создать задание на обработку чека: {jobResult.ErrorMessage}");
            }

            var jobId = jobResult.Data;

            // Создаём сообщение для отправки в очередь
            var message = new CashReceiptInputMessage
            {
                IpAddress = input.IpAddress,
                JobId = jobId,
                QrSource = input.Data
            };

            // Отправляем сообщение в очередь RabbitMQ
            var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{_queueOptions.CashReceiptInputConsumerQueueName}"));
            await endpoint.Send(message, cancellationToken);

            _logger.LogInformation("Кассовый чек отправлен в очередь. JobId: {JobId}, IP: {IpAddress}, InputType: {InputType}",
                jobId, input.IpAddress, input.Metadata.InputType);

            return jobId;
        }
    }
}
