using Renoza.DbMigration.Attributes;

namespace Renoza.DbMigration.Enums
{
    /// <summary>
    /// Тип шаблона документа
    /// </summary>
    public enum DocumentTemplateType : short
    {
        /// <summary>
        /// Договор
        /// </summary>
        [DocumentTemplateTypeDetails(1, "Contract", true)]
        Contract = 1,

        /// <summary>
        /// Акт выполненных работ
        /// </summary>
        [DocumentTemplateTypeDetails(2, "Act", true)]
        Act = 2,

        /// <summary>
        /// Счёт
        /// </summary>
        [DocumentTemplateTypeDetails(3, "Invoice", true)]
        Invoice = 3,

        /// <summary>
        /// Спецификация
        /// </summary>
        [DocumentTemplateTypeDetails(4, "Specification", true)]
        Specification = 4,

        /// <summary>
        /// Доверенность
        /// </summary>
        [DocumentTemplateTypeDetails(5, "PowerOfAttorney", true)]
        PowerOfAttorney = 5,

        /// <summary>
        /// Другое
        /// </summary>
        [DocumentTemplateTypeDetails(99, "Other", true)]
        Other = 99
    }
}
