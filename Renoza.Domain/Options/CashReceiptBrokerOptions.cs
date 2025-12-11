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

        /// <summary>
        /// Имя очереди для валидации кассовых чеков
        /// </summary>
        public string CashReceiptValidationConsumerQueueName { get; set; }

        /// <summary>
        /// Имя очереди для распознавания кассовых чеков
        /// </summary>
        public string CashReceiptRecognitionConsumerQueueName { get; set; }

        /// <summary>
        /// Имя очереди для сохранения кассовых чеков
        /// </summary>
        public string CashReceiptSaveConsumerQueueName { get; set; }

        /// <summary>
        /// URL API OFD.ru для получения данных чека
        /// </summary>
        public string OfdApiUrl { get; set; } = "https://ofd.ru/api/partner/v3/receipts/GetReceipt";

        /// <summary>
        /// Токен для доступа к API OFD.ru
        /// </summary>
        public string OfdApiToken { get; set; }
    }
}
