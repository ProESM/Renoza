namespace Renoza.DbMigration.Attributes.Company
{
    /// <summary>
    /// Атрибут детализации операции компании
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class CompanyOperationDetailsAttribute : Attribute
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Наименование (код операции)
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Отображаемое наименование
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Атрибут детализации операции
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <param name="name">Наименование (код операции)</param>
        /// <param name="displayName">Отображаемое наименование</param>
        public CompanyOperationDetailsAttribute(string id, string name, string displayName)
        {
            Id = new Guid(id);
            Name = name;
            DisplayName = displayName;
        }
    }
}
