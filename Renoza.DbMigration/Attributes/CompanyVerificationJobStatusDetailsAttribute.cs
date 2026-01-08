namespace Renoza.DbMigration.Attributes
{
    /// <summary>
    /// Атрибут детализации статуса задания на верификацию компании
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class CompanyVerificationJobStatusDetailsAttribute : Attribute
    {
        /// <summary>
        /// Идентификатор
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
        public string? DisplayName { get; }

        /// <summary>
        /// Признак активности
        /// </summary>
        public bool IsActive { get; }

        /// <summary>
        /// Атрибут детализации статуса задания на верификацию компании
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <param name="code">Уникальный код</param>
        /// <param name="name">Наименование</param>
        /// <param name="displayName">Отображаемое наименование</param>
        /// <param name="isActive">Признак активности</param>
        public CompanyVerificationJobStatusDetailsAttribute(short id, string code, string name, string? displayName, bool isActive)
        {
            Id = id;
            Code = code;
            Name = name;
            DisplayName = displayName;
            IsActive = isActive;
        }
    }
}
