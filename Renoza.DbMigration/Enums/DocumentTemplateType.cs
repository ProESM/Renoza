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
        [DocumentTemplateTypeDetails(1, "contract", "Договор", "Договор подряда", true)]
        Contract = 1,

        /// <summary>
        /// Акт выполненных работ
        /// </summary>
        [DocumentTemplateTypeDetails(2, "act", "Акт", "Акт выполненных работ", true)]
        Act = 2,

        /// <summary>
        /// Счёт
        /// </summary>
        [DocumentTemplateTypeDetails(3, "invoice", "Счёт", "Счёт на оплату", true)]
        Invoice = 3,

        /// <summary>
        /// Спецификация
        /// </summary>
        [DocumentTemplateTypeDetails(4, "specification", "Спецификация", "Спецификация к договору", true)]
        Specification = 4,

        /// <summary>
        /// Доверенность
        /// </summary>
        [DocumentTemplateTypeDetails(5, "power_of_attorney", "Доверенность", "Доверенность", true)]
        PowerOfAttorney = 5,

        /// <summary>
        /// Другое
        /// </summary>
        [DocumentTemplateTypeDetails(99, "other", "Другое", "Другое", true)]
        Other = 99
    }
}
