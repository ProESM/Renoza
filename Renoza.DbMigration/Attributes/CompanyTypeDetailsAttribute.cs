namespace Renoza.DbMigration.Attributes
{
    /// <summary>
    /// Атрибут для описания типа компании
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class CompanyTypeDetailsAttribute : Attribute
    {
        /// <summary>
        /// ID типа компании
        /// </summary>
        public short Id { get; set; }

        /// <summary>
        /// Уникальный код
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Отображаемое наименование
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// Признак активности
        /// </summary>
        public bool IsActive { get; set; }

        public CompanyTypeDetailsAttribute(short id, string code, string name, string displayName, bool isActive = true)
        {
            Id = id;
            Code = code;
            Name = name;
            DisplayName = displayName;
            IsActive = isActive;
        }
    }
}
