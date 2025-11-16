using Renoza.Infrastructure.Entities.Base;

namespace Renoza.Infrastructure.Entities.Renoza
{
    /// <summary>
    /// Связь пользователя с ролью
    /// </summary>
    public class UserRoleDao : EntityDao
    {
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// Идентификатор роли
        /// </summary>
        public Guid RoleId { get; set; }
        /// <summary>
        /// Признак активности роли для пользователя
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// Дата и время создания связи
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
        /// Навигационное свойство к роли
        /// </summary>
        public virtual RoleDao Role { get; set; } = null!;
    }
}
