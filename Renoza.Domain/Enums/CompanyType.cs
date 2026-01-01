namespace Renoza.Domain.Enums
{
    /// <summary>
    /// Тип организации (согласно DaData API)
    /// </summary>
    public enum CompanyType
    {
        /// <summary>
        /// Неизвестный тип
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Юридическое лицо
        /// </summary>
        Legal = 1,

        /// <summary>
        /// Индивидуальный предприниматель
        /// </summary>
        Individual = 2
    }
}
