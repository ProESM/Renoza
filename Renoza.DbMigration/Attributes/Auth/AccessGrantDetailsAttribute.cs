namespace Renoza.DbMigration.Attributes.Auth
{
    /// <summary>
    /// Атрибут детализации гранта доступа
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class AccessGrantDetailsAttribute : Attribute
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Наименование (код гранта)
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Отображаемое наименование
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Атрибут детализации гранта доступа
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <param name="name">Наименование (код гранта)</param>
        /// <param name="displayName">Отображаемое наименование</param>
        public AccessGrantDetailsAttribute(string id, string name, string displayName)
        {
            Id = new Guid(id);
            Name = name;
            DisplayName = displayName;
        }
    }
}
