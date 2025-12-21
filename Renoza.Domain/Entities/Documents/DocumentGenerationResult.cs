namespace Renoza.Domain.Entities.Documents
{
    /// <summary>
    /// Результат генерации документа
    /// </summary>
    public class DocumentGenerationResult
    {
        /// <summary>
        /// Идентификатор сгенерированного документа
        /// </summary>
        public Guid DocumentId { get; set; }

        /// <summary>
        /// Название документа
        /// </summary>
        public string DocumentName { get; set; } = string.Empty;

        /// <summary>
        /// Ссылка на файл документа в S3
        /// </summary>
        public string FileUrl { get; set; } = string.Empty;

        /// <summary>
        /// Размер файла в байтах
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// Формат документа
        /// </summary>
        public string FormatName { get; set; } = string.Empty;

        /// <summary>
        /// Расширение файла
        /// </summary>
        public string FileExtension { get; set; } = string.Empty;

        /// <summary>
        /// Дата и время создания
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
