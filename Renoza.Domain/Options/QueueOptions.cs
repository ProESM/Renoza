namespace Renoza.Domain.Options
{
    /// <summary>
    /// Настройки очередей
    /// </summary>
    public class QueueOptions
    {
        /// <summary>
        /// Имя очереди для входных кассовых чеков
        /// </summary>
        public string CashReceiptInputConsumerQueueName { get; set; }
    }
}
