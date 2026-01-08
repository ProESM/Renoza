namespace Renoza.DbMigration.Attributes.Company
{
    /// <summary>
    /// Атрибут детализации роли участника компании
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class CompanyMemberRoleDetailsAttribute : Attribute
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; }

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
        /// Признак системной роли
        /// </summary>
        public bool IsSystemRole { get; }

        /// <summary>
        /// Признак активности
        /// </summary>
        public bool IsActive { get; }

        /// <summary>
        /// Атрибут детализации роли участника компании
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <param name="code">Уникальный код</param>
        /// <param name="name">Наименование</param>
        /// <param name="displayName">Отображаемое наименование</param>
        /// <param name="isSystemRole">Признак системной роли</param>
        /// <param name="isActive">Признак активности</param>
        public CompanyMemberRoleDetailsAttribute(string id, string code, string name, string displayName, bool isSystemRole, bool isActive)
        {
            Id = new Guid(id);
            Code = code;
            Name = name;
            DisplayName = displayName;
            IsSystemRole = isSystemRole;
            IsActive = isActive;
        }
    }
}
