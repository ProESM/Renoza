namespace Renoza.Domain.Enums
{
    /// <summary>
    /// Формат документа
    /// </summary>
    public enum DocumentFormat : short
    {
        /// <summary>
        /// Microsoft Word (.docx)
        /// </summary>
        Docx = 1,

        /// <summary>
        /// Rich Text Format (.rtf)
        /// </summary>
        Rtf = 2,

        /// <summary>
        /// Portable Document Format (.pdf)
        /// </summary>
        Pdf = 3
    }
}
