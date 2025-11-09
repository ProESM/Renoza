using Renoza.Infrastructure.Entities.Base;

namespace Renoza.Infrastructure.Entities.Renoza
{
    /// <summary>
    /// Пароль пользователя
    /// </summary>
    public class UserPasswordDao : EntityWithIdDao<long>
    {
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// Пользователь
        /// </summary>
        public virtual UserDao User { get; set; } = null!;
        /// <summary>
        /// Хеш пароля
        /// </summary>
        public string PasswordHash { get; set; } = string.Empty;
        /// <summary>
        /// Соль пароля
        /// </summary>
        public string PasswordSalt { get; set; } = string.Empty;
        /// <summary>
        /// Признак активности
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// Дата и время создания
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        /// <summary>
        /// Дата и время срока действия
        /// </summary>
        public DateTime? ExpiredAt { get; set; } = DateTime.Now;
    }
}
