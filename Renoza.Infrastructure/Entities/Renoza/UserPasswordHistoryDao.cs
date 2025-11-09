using Renoza.Infrastructure.Entities.Base;

namespace Renoza.Infrastructure.Entities.Renoza
{
    /// <summary>
    /// История паролей пользователя
    /// </summary>
    public class UserPasswordHistoryDao : EntityWithIdDao<long>
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
        /// Дата и время начала действия
        /// </summary>
        public DateTime UsedFromAt { get; set; } = DateTime.Now;
        /// <summary>
        /// Дата и время окончания действия
        /// </summary>
        public DateTime? UsedToAt { get; set; } = DateTime.Now;
        /// <summary>
        /// Дата и время создания
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
