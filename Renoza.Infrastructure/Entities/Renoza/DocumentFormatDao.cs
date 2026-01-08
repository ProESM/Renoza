using Renoza.Infrastructure.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace Renoza.Infrastructure.Entities.Renoza
{
    /// <summary>
    /// Формат документа (справочник)
    /// </summary>
    public class DocumentFormatDao : EntityWithIdDao<short>
    {
        /// <summary>
        /// Уникальный код
        /// </summary>
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Наименование формата
        /// </summary>
        [MaxLength(256)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Отображаемое наименование
        /// </summary>
        [MaxLength(256)]
        public string? DisplayName { get; set; }

        /// <summary>
        /// Расширение файла (например: .docx, .rtf, .pdf)
        /// </summary>
        [MaxLength(10)]
        public string FileExtension { get; set; } = string.Empty;

        /// <summary>
        /// Признак активности
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Дата и время создания
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Навигационное свойство: связи с документами
        /// </summary>
        public virtual ICollection<DocumentDao> Documents { get; set; } = null!;
    }
}
