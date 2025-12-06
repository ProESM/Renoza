namespace Renoza.CashReceiptBroker
{
    /// <summary>
    /// Брокер обработки входящих кассовых чеков
    /// </summary>
    public class CashReceiptBroker : BackgroundService
    {
        /// <summary>
        /// Логгер
        /// </summary>
        private readonly ILogger<CashReceiptBroker> _logger;

        /// <summary>
        /// Брокер обработки входящих кассовых чеков
        /// </summary>
        /// <param name="logger">Логгер</param>
        public CashReceiptBroker(ILogger<CashReceiptBroker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("CashReceiptBroker запущен");
            try
            {
                await Task.Delay(1000, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка в CashReceiptBroker");
            }
        }
    }
}
