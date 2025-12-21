using Renoza.Infrastructure.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace Renoza.Infrastructure.Entities.Renoza
{
    /// <summary>
    /// Шаблон документа
    /// </summary>
    public class DocumentTemplateDao : EntityWithIdDao<Guid>
    {
        /// <summary>
        /// Идентификатор типа шаблона
        /// </summary>
        public short TemplateTypeId { get; set; }

        /// <summary>
        /// Тип шаблона
        /// </summary>
        public virtual DocumentTemplateTypeDao TemplateType { get; set; } = null!;

        /// <summary>
        /// Название шаблона
        /// </summary>
        [MaxLength(500)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Описание шаблона
        /// </summary>
        [MaxLength(2000)]
        public string? Description { get; set; }

        /// <summary>
        /// Ссылка на файл шаблона в S3
        /// </summary>
        [MaxLength(500)]
        public string FileUrl { get; set; } = string.Empty;

        /// <summary>
        /// Размер файла в байтах
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// Список доступных плейсхолдеров в формате JSON
        /// Пример: ["CustomerName", "ContractNumber", "ContractDate"]
        /// </summary>
        public string? AvailablePlaceholders { get; set; }

        /// <summary>
        /// Идентификатор пользователя, создавшего шаблон
        /// </summary>
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// Признак активности
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Дата и время создания
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Дата и время редактирования
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Навигационное свойство: связи с сгенерированными документами
        /// </summary>
        public virtual ICollection<DocumentDao> Documents { get; set; } = null!;
    }
}
