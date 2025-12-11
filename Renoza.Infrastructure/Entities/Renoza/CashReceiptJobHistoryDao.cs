using Renoza.Infrastructure.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace Renoza.Infrastructure.Entities.Renoza
{
    /// <summary>
    /// История изменений статусов запроса на загрузку чека
    /// </summary>
    public class CashReceiptJobHistoryDao : EntityWithIdDao<long>
    {
        /// <summary>
        /// Идентификатор запроса на загрузку чека
        /// </summary>
        public Guid JobId { get; set; }

        /// <summary>
        /// Запрос на загрузку чека
        /// </summary>
        public virtual CashReceiptJobDao Job { get; set; } = null!;

        /// <summary>
        /// Идентификатор статуса
        /// </summary>
        public short StatusId { get; set; }

        /// <summary>
        /// Статус
        /// </summary>
        public virtual CashReceiptJobStatusDao Status { get; set; } = null!;

        /// <summary>
        /// Комментарий к изменению статуса
        /// </summary>
        [MaxLength(2000)]
        public string? Comment { get; set; }

        /// <summary>
        /// Дата и время создания записи
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
