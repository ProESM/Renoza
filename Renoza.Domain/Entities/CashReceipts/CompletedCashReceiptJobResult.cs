namespace Renoza.Domain.Entities.CashReceipts
{
    /// <summary>
    /// Результат поиска завершенного задания по QR коду
    /// </summary>
    public class CompletedCashReceiptJobResult
    {
        /// <summary>
        /// Идентификатор завершенного задания
        /// </summary>
        public Guid JobId { get; set; }

        /// <summary>
        /// JSON данные чека от OFD API
        /// </summary>
        public string ReceiptJsonData { get; set; } = string.Empty;
    }
}
