using Renoza.DbMigration.Attributes;

namespace Renoza.DbMigration.Enums
{
    /// <summary>
    /// Формат документа
    /// </summary>
    public enum DocumentFormat : short
    {
        /// <summary>
        /// DOCX (Microsoft Word Open XML)
        /// </summary>
        [DocumentFormatDetails(1, "docx", "DOCX", "Microsoft Word документ", ".docx", true)]
        Docx = 1,

        /// <summary>
        /// RTF (Rich Text Format)
        /// </summary>
        [DocumentFormatDetails(2, "rtf", "RTF", "Rich Text Format документ", ".rtf", true)]
        Rtf = 2,

        /// <summary>
        /// PDF (Portable Document Format)
        /// </summary>
        [DocumentFormatDetails(3, "pdf", "PDF", "PDF документ", ".pdf", true)]
        Pdf = 3
    }
}
