namespace Renoza.DbMigration.Attributes.Auth
{
    /// <summary>
    /// Атрибут детализации ресурса
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class ResourceDetailsAttribute : Attribute
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Наименование (код ресурса)
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Отображаемое наименование
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Идентификатор родительского ресурса
        /// </summary>
        public Guid? ParentId { get; }

        /// <summary>
        /// Атрибут детализации ресурса
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <param name="name">Наименование (код ресурса)</param>
        /// <param name="displayName">Отображаемое наименование</param>
        /// <param name="parentId">Идентификатор родительского ресурса</param>
        public ResourceDetailsAttribute(string id, string name, string displayName, string? parentId = null)
        {
            Id = new Guid(id);
            Name = name;
            DisplayName = displayName;
            ParentId = parentId != null ? new Guid(parentId) : null;
        }
    }
}
