using Renoza.DbMigration.Attributes;

namespace Renoza.DbMigration.Enums
{
    /// <summary>
    /// Тип организации (для миграций)
    /// </summary>
    public enum CompanyType
    {
        /// <summary>
        /// Неизвестный тип
        /// </summary>
        [CompanyTypeDetails(0, "Неизвестно", true)]
        Unknown = 0,

        /// <summary>
        /// Юридическое лицо
        /// </summary>
        [CompanyTypeDetails(1, "Юридическое лицо", true)]
        Legal = 1,

        /// <summary>
        /// Индивидуальный предприниматель
        /// </summary>
        [CompanyTypeDetails(2, "Индивидуальный предприниматель", true)]
        Individual = 2
    }
}
