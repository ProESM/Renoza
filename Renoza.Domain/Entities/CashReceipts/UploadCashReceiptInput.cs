using Renoza.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Renoza.Domain.Entities.CashReceipts
{
    /// <summary>
    /// Входные данные для загрузки кассового чека
    /// </summary>
    public class UploadCashReceiptInput
    {
        /// <summary>
        /// Идентификатор пользователя, создавшего запрос
        /// </summary>
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// IP адрес клиента
        /// </summary>
        public string IpAddress { get; set; } = string.Empty;

        /// <summary>
        /// Данные чека (QR код, JSON, изображение в base64 и т.д.)
        /// </summary>
        public string Data { get; set; } = string.Empty;

        /// <summary>
        /// Поток данных файла чека.
        /// Используется для обработки содержимого файла без сохранения на диск.
        /// Может быть null, если чек загружается только через JSON.
        /// </summary>
        public Stream? FileStream { get; set; }

        /// <summary>
        /// Имя исходного файла чека.
        /// Включает расширение файла (например: "чек_123.pdf").
        /// Используется для определения типа файла и логирования.
        /// </summary>
        public string? FileName { get; set; }

        /// <summary>
        /// MIME-тип содержимого файла.
        /// Определяется из заголовков HTTP-запроса (например: "image/jpeg").
        /// Используется для валидации поддерживаемых форматов.
        /// </summary>
        public string? FileContentType { get; set; }

        /// <summary>
        /// Дополнительные метаданные кассового чека
        /// </summary>
        [Required(ErrorMessage = "Дополнительные метаданные обязательны")]
        public CashReceiptMetadata Metadata { get; set; } = null!;
    }
}
