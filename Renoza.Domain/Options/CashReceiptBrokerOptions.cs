namespace Renoza.Domain.Options
{
    /// <summary>
    /// Настройки брокера кассовых чеков
    /// </summary>
    public class CashReceiptBrokerOptions
    {
        /// <summary>
        /// Имя очереди для входных кассовых чеков
        /// </summary>
        public string CashReceiptInputConsumerQueueName { get; set; }
    }
}
