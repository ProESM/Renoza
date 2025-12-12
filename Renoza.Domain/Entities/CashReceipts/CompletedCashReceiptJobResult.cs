namespace Renoza.Domain.Entities.CashReceipts
{
    /// <summary>
    /// Результат поиска успешно обработанного чека по QR коду
    /// </summary>
    public class CompletedCashReceiptJobResult
    {
        /// <summary>
        /// Идентификатор существующего кассового чека
        /// </summary>
        public Guid CashReceiptId { get; set; }

        /// <summary>
        /// JSON данные чека от OFD API
        /// </summary>
        public string ReceiptJsonData { get; set; } = string.Empty;

        /// <summary>
        /// URL PDF файла чека в S3
        /// </summary>
        public string? PdfUrl { get; set; }
    }
}
