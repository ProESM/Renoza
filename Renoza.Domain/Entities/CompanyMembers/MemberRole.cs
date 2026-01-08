namespace Renoza.Domain.Entities.CompanyMembers
{
    /// <summary>
    /// Роль участника компании
    /// </summary>
    public class MemberRole
    {
        /// <summary>
        /// Идентификатор роли
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Уникальный код роли
        /// </summary>
        public string Code { get; set; } = null!;

        /// <summary>
        /// Наименование роли
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Отображаемое наименование
        /// </summary>
        public string? DisplayName { get; set; }

        /// <summary>
        /// Признак системной роли (не может быть удалена)
        /// </summary>
        public bool IsSystemRole { get; set; }

        /// <summary>
        /// Признак активности
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
