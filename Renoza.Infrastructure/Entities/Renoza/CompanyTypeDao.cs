using Renoza.Infrastructure.Entities.Base;

namespace Renoza.Infrastructure.Entities.Renoza
{
    /// <summary>
    /// Справочник типов компаний
    /// </summary>
    public class CompanyTypeDao : EntityWithIdDao<short>
    {
        /// <summary>
        /// Название типа компании
        /// </summary>
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
        /// Навигационное свойство к профилям компаний
        /// </summary>
        public virtual ICollection<CompanyProfileDao> CompanyProfiles { get; set; } = null!;
    }
}
