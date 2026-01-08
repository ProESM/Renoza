using Renoza.Infrastructure.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace Renoza.Infrastructure.Entities.Renoza
{
    /// <summary>
    /// Роль участника компании (Owner, Manager, Employee)
    /// </summary>
    public class MemberRoleDao : EntityWithIdDao<Guid>
    {
        /// <summary>
        /// Уникальный код
        /// </summary>
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Наименование
        /// </summary>
        [MaxLength(256)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Отображаемое наименование
        /// </summary>
        [MaxLength(256)]
        public string? DisplayName { get; set; }

        /// <summary>
        /// Признак системной роли
        /// </summary>
        public bool IsSystemRole { get; set; }

        /// <summary>
        /// Признак активности
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Дата и время создания
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Навигационное свойство к участникам компаний с этой ролью
        /// </summary>
        public virtual ICollection<CompanyMemberDao> CompanyMembers { get; set; } = null!;
    }
}
