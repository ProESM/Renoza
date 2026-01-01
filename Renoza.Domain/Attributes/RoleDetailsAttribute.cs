namespace Renoza.Domain.Attributes
{
    /// <summary>
    /// Атрибут детализации роли
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class RoleDetailsAttribute : Attribute
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; }
        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Описание
        /// </summary>
        public string Description { get; }
        /// <summary>
        /// Признак системной роли (не может быть удалена)
        /// </summary>
        public bool IsSystemRole { get; }
        /// <summary>
        /// Признак активности
        /// </summary>
        public bool IsActive { get; }

        /// <summary>
        /// Атрибут детализации роли
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <param name="name">Наименование</param>
        /// <param name="description">Описание</param>
        /// <param name="isSystemRole">Признак системной роли (не может быть удалена)</param>
        /// <param name="isActive">Признак активности</param>
        public RoleDetailsAttribute(string id, string name, string description, bool isSystemRole, bool isActive)
        {
            Id = new Guid(id);
            Name = name;
            Description = description;
            IsSystemRole = isSystemRole;
            IsActive = isActive;
        }
    }
}
