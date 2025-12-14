using Microsoft.AspNetCore.Mvc;
using Renoza.Domain.Entities.CashReceipts;
using System.ComponentModel.DataAnnotations;

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
        [FromForm]
        public string Data { get; set; } = string.Empty;

        /// <summary>
        /// Файл чека (опционально, но если нет файла, должен быть Data)
        /// </summary>
        [FromForm]
        public IFormFile? File { get; set; }

        /// <summary>
        /// Дополнительные метаданные кассового чека
        /// </summary>
        [FromForm]
        [Required(ErrorMessage = "Дополнительные метаданные обязательны")]
        public CashReceiptMetadata Metadata { get; set; } = null!;
    }
}
