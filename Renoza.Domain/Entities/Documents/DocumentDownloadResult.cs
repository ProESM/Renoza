namespace Renoza.Domain.Entities.Documents
{
    /// <summary>
    /// Результат скачивания документа
    /// </summary>
    public class DocumentDownloadResult
    {
        /// <summary>
        /// Поток с данными файла
        /// </summary>
        public Stream FileStream { get; set; } = null!;

        /// <summary>
        /// Название документа
        /// </summary>
        public string DocumentName { get; set; } = string.Empty;

        /// <summary>
        /// Расширение файла
        /// </summary>
        public string FileExtension { get; set; } = string.Empty;
    }
}
