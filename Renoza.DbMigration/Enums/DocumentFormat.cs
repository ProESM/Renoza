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
        [DocumentFormatDetails(1, "DOCX", ".docx", true)]
        Docx = 1,

        /// <summary>
        /// RTF (Rich Text Format)
        /// </summary>
        [DocumentFormatDetails(2, "RTF", ".rtf", true)]
        Rtf = 2,

        /// <summary>
        /// PDF (Portable Document Format)
        /// </summary>
        [DocumentFormatDetails(3, "PDF", ".pdf", true)]
        Pdf = 3
    }
}
