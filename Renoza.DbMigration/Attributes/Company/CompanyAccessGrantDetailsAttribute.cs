namespace Renoza.DbMigration.Attributes.Company
{
    /// <summary>
    /// Атрибут детализации гранта доступа компании
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class CompanyAccessGrantDetailsAttribute : Attribute
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
        /// Атрибут детализации гранта доступа компании
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <param name="name">Наименование (код гранта)</param>
        /// <param name="displayName">Отображаемое наименование</param>
        public CompanyAccessGrantDetailsAttribute(string id, string name, string displayName)
        {
            Id = new Guid(id);
            Name = name;
            DisplayName = displayName;
        }
    }
}
