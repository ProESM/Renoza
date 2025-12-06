using MassTransit;
using Microsoft.Extensions.Logging;
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
        /// Сервис для работы с чеками
        /// </summary>
        /// <param name="bus">Шина сообщений</param>
        /// <param name="logger">Логгер</param>
        /// <param name="queueOptions">Настройки очередей</param>
        /// <param name="rateLimitService">Сервис для проверки Rate Limiting</param>
        public ReceiptService(IBus bus,
            ILogger<ReceiptService> logger,
            QueueOptions queueOptions,
            IRateLimitService rateLimitService)
        {
            _bus = bus;
            _logger = logger;
            _queueOptions = queueOptions;
            _rateLimitService = rateLimitService;
        }

        /// <summary>
        /// Загрузить кассовый чек (отправить в очередь обработки)
        /// </summary>
        /// <param name="ipAddress">IP адрес клиента</param>
        /// <param name="inputType">Тип входных данных</param>
        /// <param name="data">Данные чека</param>
        /// <param name="contentType">MIME тип файла (опционально)</param>
        /// <param name="fileName">Имя файла (опционально)</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Идентификатор задания на обработку</returns>
        public async Task<Guid> UploadCashReceiptAsync(
            string ipAddress,
            ReceiptInputType inputType,
            string data,
            string? contentType = null,
            string? fileName = null,
            CancellationToken cancellationToken = default)
        {
            // Проверка Rate Limiting
            var isAllowed = await _rateLimitService.IsAllowedAsync(ipAddress, cancellationToken);
            if (!isAllowed)
            {
                _logger.LogWarning($"Запрос отклонён из-за превышения rate limit. IP: {ipAddress}");

                // Получаем информацию о лимите для логирования
                var rateLimitInfo = await _rateLimitService.GetRateLimitInfoAsync(ipAddress);
                _logger.LogWarning($"Rate limit для IP {ipAddress}: {rateLimitInfo.CurrentCount}/{rateLimitInfo.Limit} за {rateLimitInfo.WindowDuration.TotalSeconds}с");

                // Отклоняем запрос
                throw new RateLimitExceededException(ipAddress, rateLimitInfo.CurrentCount, rateLimitInfo.Limit);
            }

            // Генерируем уникальный ID задания
            var jobId = Guid.NewGuid();

            // Создаём сообщение для отправки в очередь
            var message = new CashReceiptInputMessage
            {
                IpAddress = ipAddress,
                JobId = jobId
            };

            // Отправляем сообщение в очередь RabbitMQ
            var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{_queueOptions.CashReceiptInputConsumerQueueName}"));
            await endpoint.Send(message);

            _logger.LogInformation("Кассовый чек отправлен в очередь. JobId: {JobId}, IP: {IpAddress}, InputType: {InputType}",
                jobId, ipAddress, inputType);

            return jobId;
        }
    }
}
