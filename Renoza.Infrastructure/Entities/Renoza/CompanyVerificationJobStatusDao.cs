using Renoza.Infrastructure.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace Renoza.Infrastructure.Entities.Renoza
{
    /// <summary>
    /// Справочник статусов заданий на верификацию компании
    /// </summary>
    public class CompanyVerificationJobStatusDao : EntityWithIdDao<short>
    {
        /// <summary>
        /// Название статуса
        /// </summary>
        [MaxLength(256)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Признак активности
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Дата обновления
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Навигационное свойство к заданиям
        /// </summary>
        public virtual ICollection<CompanyVerificationJobDao> Jobs { get; set; } = null!;
    }
}
