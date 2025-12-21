namespace Renoza.Domain.Enums
{
    /// <summary>
    /// Тип шаблона документа
    /// </summary>
    public enum DocumentTemplateType : short
    {
        /// <summary>
        /// Договор
        /// </summary>
        Contract = 1,

        /// <summary>
        /// Акт выполненных работ
        /// </summary>
        Act = 2,

        /// <summary>
        /// Счёт
        /// </summary>
        Invoice = 3,

        /// <summary>
        /// Спецификация
        /// </summary>
        Specification = 4,

        /// <summary>
        /// Доверенность
        /// </summary>
        PowerOfAttorney = 5,

        /// <summary>
        /// Прочее
        /// </summary>
        Other = 99
    }
}
