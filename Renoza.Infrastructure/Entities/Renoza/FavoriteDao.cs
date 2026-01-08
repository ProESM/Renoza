using Renoza.Infrastructure.Entities.Base;

namespace Renoza.Infrastructure.Entities.Renoza
{
    /// <summary>
    /// DAO сущность для избранного
    /// </summary>
    public class FavoriteDao : EntityWithIdDao<Guid>
    {
        /// <summary>
        /// ID пользователя, который добавил в избранное
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// ID профиля, который добавлен в избранное
        /// </summary>
        public Guid ProfileId { get; set; }

        /// <summary>
        /// Дата добавления в избранное
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Навигационное свойство к пользователю
        /// </summary>
        public virtual UserDao User { get; set; } = null!;
    }
}
