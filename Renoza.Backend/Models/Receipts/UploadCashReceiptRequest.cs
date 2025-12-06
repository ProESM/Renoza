using Renoza.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Renoza.Backend.Models.Receipts
{
    /// <summary>
    /// Запрос на отправку кассового чека в очередь обработки
    /// </summary>
    public class UploadCashReceiptRequest
    {
        /// <summary>
        /// Тип входных данных
        /// </summary>
        [Required(ErrorMessage = "Тип входных данных обязателен")]
        public ReceiptInputType InputType { get; set; }

        /// <summary>
        /// Данные чека (QR код, base64 изображения и т.д.)
        /// </summary>
        [Required(ErrorMessage = "Данные чека обязательны")]
        public string Data { get; set; } = string.Empty;

        /// <summary>
        /// MIME тип файла (для изображений и PDF)
        /// </summary>
        public string? ContentType { get; set; }

        /// <summary>
        /// Имя файла (опционально)
        /// </summary>
        public string? FileName { get; set; }
    }
}
