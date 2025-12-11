using Renoza.Domain.Enums;

namespace Renoza.Domain.Entities.CashReceipts
{
    /// <summary>
    /// Входные данные для загрузки кассового чека
    /// </summary>
    public class UploadCashReceiptInput
    {
        /// <summary>
        /// Идентификатор заказчика
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// Идентификатор пользователя, создавшего запрос
        /// </summary>
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// IP адрес клиента
        /// </summary>
        public string IpAddress { get; set; } = string.Empty;

        /// <summary>
        /// Тип входных данных
        /// </summary>
        public ReceiptInputType InputType { get; set; }

        /// <summary>
        /// Данные чека (QR код, JSON, изображение в base64 и т.д.)
        /// </summary>
        public string Data { get; set; } = string.Empty;

        /// <summary>
        /// Идентификатор заказа (опционально)
        /// </summary>
        public Guid? OrderId { get; set; }

        /// <summary>
        /// MIME тип файла (опционально)
        /// </summary>
        public string? ContentType { get; set; }

        /// <summary>
        /// Имя файла (опционально)
        /// </summary>
        public string? FileName { get; set; }
    }
}
