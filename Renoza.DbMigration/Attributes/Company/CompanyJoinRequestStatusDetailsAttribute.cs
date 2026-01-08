namespace Renoza.DbMigration.Attributes.Company
{
    /// <summary>
    /// Атрибут для описания статуса запроса на вступление в компанию
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class CompanyJoinRequestStatusDetailsAttribute : Attribute
    {
        /// <summary>
        /// ID статуса
        /// </summary>
        public short Id { get; }

        /// <summary>
        /// Уникальный код
        /// </summary>
        public string Code { get; }

        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Отображаемое наименование
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Признак активности
        /// </summary>
        public bool IsActive { get; }

        /// <summary>
        /// Конструктор атрибута
        /// </summary>
        /// <param name="id">ID статуса</param>
        /// <param name="code">Уникальный код</param>
        /// <param name="name">Наименование</param>
        /// <param name="displayName">Отображаемое наименование</param>
        /// <param name="isActive">Признак активности</param>
        public CompanyJoinRequestStatusDetailsAttribute(short id, string code, string name, string displayName, bool isActive)
        {
            Id = id;
            Code = code;
            Name = name;
            DisplayName = displayName;
            IsActive = isActive;
        }
    }
}
