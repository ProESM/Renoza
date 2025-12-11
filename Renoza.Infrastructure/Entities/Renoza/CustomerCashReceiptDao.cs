using Renoza.Infrastructure.Entities.Base;

namespace Renoza.Infrastructure.Entities.Renoza
{
    /// <summary>
    /// Кассовые чеки заказчика
    /// </summary>
    public class CustomerCashReceiptDao : EntityDao
    {
        /// <summary>
        /// Идентификатор заказчика
        /// </summary>
        public Guid CustomerId { get; set; }
        /// <summary>
        /// Идентификатор кассового чека
        /// </summary>
        public Guid CashReceiptId { get; set; }
        /// <summary>
        /// Идентификатор заказа (опционально)
        /// </summary>
        public Guid? OrderId { get; set; }
        /// <summary>
        /// Дата и время создания
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Заказчик
        /// </summary>
        public virtual CustomerProfileDao Customer { get; set; } = null!;

        /// <summary>
        /// Кассовый чек
        /// </summary>
        public virtual CashReceiptDao CashReceipt { get; set; } = null!;
    }
}
