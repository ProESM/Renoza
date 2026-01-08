using Renoza.Infrastructure.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace Renoza.Infrastructure.Entities.Renoza
{
    /// <summary>
    /// Статус документа (справочник)
    /// </summary>
    public class DocumentStatusDao : EntityWithIdDao<short>
    {
        /// <summary>
        /// Уникальный код
        /// </summary>
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Наименование статуса
        /// </summary>
        [MaxLength(256)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Отображаемое наименование
        /// </summary>
        [MaxLength(256)]
        public string? DisplayName { get; set; }

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
