using Renoza.Infrastructure.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace Renoza.Infrastructure.Entities.Renoza
{
    /// <summary>
    /// Участник компании (связь пользователя с компанией)
    /// </summary>
    public class CompanyMemberDao : EntityWithIdDao<Guid>
    {
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Идентификатор профиля компании
        /// </summary>
        public Guid CompanyProfileId { get; set; }

        /// <summary>
        /// Идентификатор роли участника в компании
        /// </summary>
        public Guid MemberRoleId { get; set; }

        /// <summary>
        /// Должность в компании
        /// </summary>
        [MaxLength(256)]
        public string? Position { get; set; }

        /// <summary>
        /// Дата вступления в компанию
        /// </summary>
        public DateTime JoinedAt { get; set; }

        /// <summary>
        /// Дата выхода из компании
        /// </summary>
        public DateTime? LeftAt { get; set; }

        /// <summary>
        /// Признак активности участника в компании
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Дата и время создания записи
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Дата и время редактирования записи
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Навигационное свойство к пользователю
        /// </summary>
        public virtual UserDao User { get; set; } = null!;

        /// <summary>
        /// Навигационное свойство к профилю компании
        /// </summary>
        public virtual CompanyProfileDao CompanyProfile { get; set; } = null!;

        /// <summary>
        /// Навигационное свойство к роли участника компании
        /// </summary>
        public virtual MemberRoleDao MemberRole { get; set; } = null!;
    }
}
