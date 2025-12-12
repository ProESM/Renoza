using Renoza.Domain.Enums;

namespace Renoza.Domain.Entities.CashReceipts
{
    /// <summary>
    /// Входные данные для сохранения кассового чека
    /// </summary>
    public class SaveCashReceiptInput
    {
        /// <summary>
        /// Идентификатор задания
        /// </summary>
        public Guid JobId { get; set; }

        /// <summary>
        /// Тип входных данных
        /// </summary>
        public ReceiptInputType InputType { get; set; }

        /// <summary>
        /// Данные чека (JSON или другой формат)
        /// </summary>
        public string Data { get; set; } = string.Empty;

        /// <summary>
        /// MIME тип файла (опционально)
        /// </summary>
        public string? ContentType { get; set; }

        /// <summary>
        /// Имя файла (опционально)
        /// </summary>
        public string? FileName { get; set; }

        /// <summary>
        /// Идентификатор существующего чека (опционально, для переиспользования)
        /// </summary>
        public Guid? CashReceiptId { get; set; }

        /// <summary>
        /// URL PDF файла чека в S3 (опционально, для переиспользования существующего)
        /// </summary>
        public string? PdfUrl { get; set; }
    }
}
