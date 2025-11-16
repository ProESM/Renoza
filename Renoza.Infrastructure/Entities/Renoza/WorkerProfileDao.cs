using Renoza.Infrastructure.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace Renoza.Infrastructure.Entities.Renoza
{
    /// <summary>
    /// Профиль работника
    /// </summary>
    public class WorkerProfileDao : EntityWithIdDao<Guid>
    {
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Специализация работника
        /// </summary>
        [MaxLength(256)]
        public string? Specialization { get; set; }

        /// <summary>
        /// Размер команды
        /// </summary>
        public int? TeamSize { get; set; }

        /// <summary>
        /// Сертификаты и квалификации
        /// </summary>
        public string[]? Certifications { get; set; }

        /// <summary>
        /// Дата начала профессиональной деятельности
        /// </summary>
        public DateOnly? ProfessionalStartDate { get; set; }

        /// <summary>
        /// Признак доступности для новых заказов
        /// </summary>
        public bool IsAvailable { get; set; }

        /// <summary>
        /// Рейтинг работника
        /// </summary>
        public decimal? Rating { get; set; }

        /// <summary>
        /// Признак активности профиля
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
        /// Навигационное свойство к пользователю
        /// </summary>
        public virtual UserDao User { get; set; } = null!;
    }
}
