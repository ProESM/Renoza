using System.ComponentModel.DataAnnotations;

namespace Renoza.Domain.Options
{
    /// <summary>
    /// Настройки S3 хранилища
    /// </summary>
    public class S3Options
    {
        /// <summary>
        /// URL эндпоинта S3 (для альтернативных S3-совместимых хранилищ)
        /// </summary>
        public string? ServiceUrl { get; set; }

        /// <summary>
        /// Access Key для доступа к S3
        /// </summary>
        [Required]
        public string AccessKey { get; set; } = string.Empty;

        /// <summary>
        /// Secret Key для доступа к S3
        /// </summary>
        [Required]
        public string SecretKey { get; set; } = string.Empty;

        /// <summary>
        /// Имя бакета по умолчанию
        /// </summary>
        [Required]
        public string BucketName { get; set; } = string.Empty;

        /// <summary>
        /// Регион (для AWS S3)
        /// </summary>
        public string? Region { get; set; }

        /// <summary>
        /// Использовать путь в стиле виртуального хоста (true для AWS, false для большинства альтернативных хранилищ)
        /// </summary>
        public bool ForcePathStyle { get; set; } = true;

        /// <summary>
        /// Публичный URL для доступа к файлам (если отличается от ServiceUrl)
        /// </summary>
        public string? PublicUrl { get; set; }

        /// <summary>
        /// Максимальный размер файла в байтах (по умолчанию 100 МБ)
        /// </summary>
        [Range(1, long.MaxValue)]
        public long MaxFileSize { get; set; } = 100 * 1024 * 1024;

        /// <summary>
        /// Разрешенные типы файлов (MIME types)
        /// </summary>
        public string[] AllowedContentTypes { get; set; } = Array.Empty<string>();
    }
}
