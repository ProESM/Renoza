using Renoza.Infrastructure.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace Renoza.Infrastructure.Entities.Renoza
{
    /// <summary>
    /// Профиль технического надзора
    /// </summary>
    public class TechnicalSupervisorProfileDao : EntityWithIdDao<Guid>
    {
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Специализация (виды надзора)
        /// </summary>
        [MaxLength(256)]
        public string? Specialization { get; set; }

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
        /// Рейтинг
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
