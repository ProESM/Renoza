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
        /// Название типа компании
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Признак активности
        /// </summary>
        public bool IsActive { get; set; }

        public CompanyTypeDetailsAttribute(short id, string name, bool isActive = true)
        {
            Id = id;
            Name = name;
            IsActive = isActive;
        }
    }
}
