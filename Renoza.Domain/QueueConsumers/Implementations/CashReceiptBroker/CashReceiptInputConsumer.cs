using MassTransit;
using Microsoft.Extensions.Logging;
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
        /// Consumer для обработки входящих кассовых чеков
        /// </summary>
        /// <param name="logger">Логгер</param>
        /// <param name="bus">Шина сообщений</param>
        /// <param name="cashReceiptBrokerOptions">Настройки брокера кассовых чеков</param>
        /// <param name="rateLimitService">Сервис для проверки Rate Limiting</param>
        public CashReceiptInputConsumer(
            ILogger<CashReceiptInputConsumer> logger,
            IBus bus,
            CashReceiptBrokerOptions cashReceiptBrokerOptions,
            IRateLimitService rateLimitService)
        {
            _logger = logger;
            _bus = bus;
            _cashReceiptBrokerOptions = cashReceiptBrokerOptions;
            _rateLimitService = rateLimitService;
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

            // Проверка Rate Limiting
            var isAllowed = await _rateLimitService.IsAllowedAsync(cashReceiptInputMessage.IpAddress);
            if (!isAllowed)
            {
                _logger.LogWarning($"Сообщение отклонено из-за превышения rate limit. IP: {cashReceiptInputMessage.IpAddress}, JobId: {cashReceiptInputMessage.JobId}");

                // Получаем информацию о лимите для логирования
                var rateLimitInfo = await _rateLimitService.GetRateLimitInfoAsync(cashReceiptInputMessage.IpAddress);
                _logger.LogWarning($"Rate limit для IP {cashReceiptInputMessage.IpAddress}: {rateLimitInfo.CurrentCount}/{rateLimitInfo.Limit} за {rateLimitInfo.WindowDuration.TotalSeconds}с");

                // Отклоняем сообщение - оно попадёт в Dead Letter Queue
                throw new RateLimitExceededException(cashReceiptInputMessage.IpAddress, rateLimitInfo.CurrentCount, rateLimitInfo.Limit);
            }

            _logger.LogInformation($"Сообщение принято для обработки. IP: {cashReceiptInputMessage.IpAddress}, JobId: {cashReceiptInputMessage.JobId}");

            // Здесь будет основная логика обработки кассового чека
            //_logger.LogInformation($"Получено задание на расчёт {externalPromoForecastCalculationJobMessage.JobId} с количеством задач: {externalPromoForecastCalculationJobMessage.Tasks.Count}");
        }
    }
}
