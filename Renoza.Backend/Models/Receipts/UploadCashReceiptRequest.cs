using System.ComponentModel.DataAnnotations;
using Renoza.Domain.Entities.CashReceipts;

namespace Renoza.Backend.Models.Receipts
{
    /// <summary>
    /// Запрос на отправку кассового чека в очередь обработки
    /// </summary>
    public class UploadCashReceiptRequest
    {
        /// <summary>
        /// Данные чека (QR код, base64 изображения и т.д.)
        /// </summary>
        public string Data { get; set; } = string.Empty;

        /// <summary>
        /// Файл чека (опционально, но если нет файла, должен быть Data)
        /// </summary>
        public IFormFile? File { get; set; }

        /// <summary>
        /// Дополнительные метаданные кассового чека
        /// </summary>
        [Required(ErrorMessage = "Дополнительные метаданные обязательны")]
        public CashReceiptMetadata Metadata { get; set; } = null!;
    }
}
