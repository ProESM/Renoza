namespace Renoza.DbMigration.Attributes
{
    /// <summary>
    /// Атрибут детализации страны
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class CountryDetailsAttribute : Attribute
    {
        /// <summary>
        /// Код
        /// </summary>
        public string Code { get; }
        /// <summary>
        /// Международный телефонный код
        /// </summary>
        public string PhoneCountryCode { get; }
        /// <summary>
        /// Формат телефонного номера
        /// </summary>
        public string PhoneFormat { get; }
        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Признак активности
        /// </summary>
        public bool IsActive { get; }

        /// <summary>
        /// Атрибут детализации страны
        /// </summary>
        /// <param name="code">Код</param>
        /// <param name="name">Наименование</param>
        /// <param name="phoneCountryCode">Международный телефонный код</param>
        /// <param name="phoneFormat">Формат телефонного номера</param>
        /// <param name="isActive">Признак активности</param>
        public CountryDetailsAttribute(string code, string name, string phoneCountryCode, string phoneFormat, bool isActive)
        {
            Code = code;
            Name = name;
            PhoneCountryCode = phoneCountryCode;
            PhoneFormat = phoneFormat;
            IsActive = isActive;
        }
    }
}
