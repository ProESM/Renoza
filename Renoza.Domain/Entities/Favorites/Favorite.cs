namespace Renoza.Domain.Entities.Favorites
{
    /// <summary>
    /// Избранное - связь между пользователем и профилем, который он добавил в избранное
    /// </summary>
    public class Favorite
    {
        /// <summary>
        /// Идентификатор записи избранного
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// ID пользователя, который добавил в избранное
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// ID профиля, который добавлен в избранное (Profile - базовая таблица для TPT)
        /// </summary>
        public Guid ProfileId { get; set; }

        /// <summary>
        /// Дата добавления в избранное
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
