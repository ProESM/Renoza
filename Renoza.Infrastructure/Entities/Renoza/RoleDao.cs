using Renoza.Infrastructure.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace Renoza.Infrastructure.Entities.Renoza
{
    /// <summary>
    /// Роль пользователя
    /// </summary>
    public class RoleDao : EntityWithIdDao<Guid>
    {
        /// <summary>
        /// Наименование роли
        /// </summary>
        [MaxLength(256)]
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Описание роли
        /// </summary>
        [MaxLength(256)]
        public string? Description { get; set; }
        /// <summary>
        /// Признак системной роли (не может быть удалена)
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
        /// Дата и время редактирования
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Навигационное свойство: связи с пользователями
        /// </summary>
        public virtual ICollection<UserRoleDao> UserRoles { get; set; } = null!;
    }
}
