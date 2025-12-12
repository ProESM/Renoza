using Renoza.Infrastructure.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace Renoza.Infrastructure.Entities.Renoza
{
    /// <summary>
    /// Запрос на загрузку чека
    /// </summary>
    public class CashReceiptJobDao : EntityWithIdDao<Guid>
    {
        /// <summary>
        /// Идентификатор клиента
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// Идентификатор пользователя, создавшего запрос
        /// </summary>
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// Идентификатор заказа (опционально)
        /// </summary>
        public Guid? OrderId { get; set; }

        /// <summary>
        /// Идентификатор статуса
        /// </summary>
        public short StatusId { get; set; }

        /// <summary>
        /// Статус
        /// </summary>
        public virtual CashReceiptJobStatusDao Status { get; set; } = null!;

        /// <summary>
        /// Комментарий к текущему статусу
        /// </summary>
        [MaxLength(2000)]
        public string? StatusComment { get; set; }

        /// <summary>
        /// QR-код или источник данных
        /// </summary>
        [MaxLength(256)]
        public string QrSource { get; set; } = string.Empty;

        /// <summary>
        /// IP адрес пользователя
        /// </summary>
        [MaxLength(50)]
        public string IpAddress { get; set; } = string.Empty;

        /// <summary>
        /// Идентификатор запроса на загрузку чека
        /// </summary>
        public Guid? CashReceiptId { get; set; }

        /// <summary>
        /// Навигационное свойство: чек (результат обработки)
        /// </summary>
        public virtual CashReceiptDao? CashReceipt { get; set; } = null!;

        /// <summary>
        /// Дата и время создания
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Дата и время редактирования
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Дата и время завершения (опционально)
        /// </summary>
        public DateTime? CompletedAt { get; set; }

        /// <summary>
        /// Навигационное свойство: история изменений статусов
        /// </summary>
        public virtual ICollection<CashReceiptJobHistoryDao> History { get; set; } = null!;
    }
}
