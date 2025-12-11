using Renoza.Infrastructure.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace Renoza.Infrastructure.Entities.Renoza
{
    /// <summary>
    /// Кассовый чек
    /// </summary>
    public class CashReceiptDao : EntityWithIdDao<Guid>
    {
        /// <summary>
        /// QR код
        /// </summary>
        [MaxLength(256)]
        public string QrCode { get; set; } = string.Empty;

        /// <summary>
        /// Полные данные чека в формате JSON
        /// </summary>
        public string? JsonData { get; set; } = string.Empty;

        /// <summary>
        /// Ссылка на PDF файл чека
        /// </summary>
        [MaxLength(500)]
        public string? PdfUrl { get; set; } = string.Empty;

        /// <summary>
        /// Общая сумма чека
        /// </summary>
        public decimal? TotalAmount { get; set; }

        /// <summary>
        /// Дата и время кассового чека
        /// </summary>
        public DateTime? DocumentDateTime { get; set; }

        /// <summary>
        /// Дата и время создания
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Навигационное свойство: связи с заданиями
        /// </summary>
        public virtual ICollection<CashReceiptJobDao> CashReceiptJobs { get; set; } = null!;

        /// <summary>
        /// Навигационное свойство: связи с заказчиками
        /// </summary>
        public virtual ICollection<CustomerCashReceiptDao> CustomerCashReceipts { get; set; } = null!;
    }
}
