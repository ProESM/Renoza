using Renoza.Infrastructure.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace Renoza.Infrastructure.Entities.Renoza
{
    /// <summary>
    /// Профиль заказчика
    /// </summary>
    public class CustomerProfileDao : EntityWithIdDao<Guid>
    {
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Название компании
        /// </summary>
        [MaxLength(256)]
        public string? CompanyName { get; set; }

        /// <summary>
        /// ИНН
        /// </summary>
        [MaxLength(50)]
        public string? TaxId { get; set; }

        /// <summary>
        /// Адрес для выставления счетов
        /// </summary>
        [MaxLength(500)]
        public string? BillingAddress { get; set; }

        /// <summary>
        /// Кредитный лимит
        /// </summary>
        public decimal? CreditLimit { get; set; }

        /// <summary>
        /// Признак активности профиля
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Дата и время создания
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Дата и время редактирования
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Навигационное свойство к пользователю
        /// </summary>
        public virtual UserDao User { get; set; } = null!;

        /// <summary>
        /// Навигационное свойство: связи с кассовыми чеками
        /// </summary>
        public virtual ICollection<CustomerCashReceiptDao> CustomerCashReceipts { get; set; } = null!;
    }
}
