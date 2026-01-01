using Renoza.Infrastructure.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace Renoza.Infrastructure.Entities.Renoza
{
    /// <summary>
    /// История изменений статусов задания на верификацию компании
    /// </summary>
    public class CompanyVerificationJobHistoryDao : EntityWithIdDao<long>
    {
        /// <summary>
        /// Идентификатор задания
        /// </summary>
        public Guid JobId { get; set; }

        /// <summary>
        /// Статус задания
        /// </summary>
        public short StatusId { get; set; }

        /// <summary>
        /// Комментарий к изменению статуса
        /// </summary>
        [MaxLength(2000)]
        public string? Comment { get; set; }

        /// <summary>
        /// Дата и время изменения
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Навигационное свойство к заданию
        /// </summary>
        public virtual CompanyVerificationJobDao Job { get; set; } = null!;

        /// <summary>
        /// Навигационное свойство к статусу
        /// </summary>
        public virtual CompanyVerificationJobStatusDao Status { get; set; } = null!;
    }
}
