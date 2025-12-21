using Renoza.Infrastructure.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace Renoza.Infrastructure.Entities.Renoza
{
    /// <summary>
    /// Сгенерированный документ
    /// </summary>
    public class DocumentDao : EntityWithIdDao<Guid>
    {
        /// <summary>
        /// Идентификатор шаблона документа
        /// </summary>
        public Guid TemplateId { get; set; }

        /// <summary>
        /// Шаблон документа
        /// </summary>
        public virtual DocumentTemplateDao Template { get; set; } = null!;

        /// <summary>
        /// Идентификатор формата документа
        /// </summary>
        public short FormatId { get; set; }

        /// <summary>
        /// Формат документа
        /// </summary>
        public virtual DocumentFormatDao Format { get; set; } = null!;

        /// <summary>
        /// Идентификатор статуса документа
        /// </summary>
        public short StatusId { get; set; }

        /// <summary>
        /// Статус документа
        /// </summary>
        public virtual DocumentStatusDao Status { get; set; } = null!;

        /// <summary>
        /// Название документа
        /// </summary>
        [MaxLength(500)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Ссылка на файл документа в S3
        /// </summary>
        [MaxLength(500)]
        public string FileUrl { get; set; } = string.Empty;

        /// <summary>
        /// Размер файла в байтах
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// Данные, использованные для генерации документа в формате JSON
        /// Пример: {"CustomerName": "ООО Рога и копыта", "ContractNumber": "123/2024"}
        /// </summary>
        public string? PlaceholderData { get; set; }

        /// <summary>
        /// Идентификатор заказа (опционально)
        /// </summary>
        public Guid? OrderId { get; set; }

        /// <summary>
        /// Идентификатор заказчика (опционально)
        /// </summary>
        public Guid? CustomerId { get; set; }

        /// <summary>
        /// Идентификатор пользователя, создавшего документ
        /// </summary>
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// Дата и время создания
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Дата и время редактирования
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Дата и время первого скачивания документа
        /// </summary>
        public DateTime? FirstDownloadedAt { get; set; }

        /// <summary>
        /// Дата и время последнего скачивания документа
        /// </summary>
        public DateTime? LastDownloadedAt { get; set; }

        /// <summary>
        /// Количество скачиваний
        /// </summary>
        public int DownloadCount { get; set; }
    }
}
