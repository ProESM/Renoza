namespace Renoza.Backend.Models.Receipts
{
    /// <summary>
    /// Ответ на отправку чека в очередь
    /// </summary>
    public class UploadReceiptResponse
    {
        /// <summary>
        /// Идентификатор задания на обработку
        /// </summary>
        public Guid JobId { get; set; }

        /// <summary>
        /// Сообщение о результате
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Время принятия в обработку
        /// </summary>
        public DateTime UploadedAt { get; set; }
    }
}
