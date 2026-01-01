using Renoza.Infrastructure.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace Renoza.Infrastructure.Entities.Renoza
{
    /// <summary>
    /// Задание на верификацию компании
    /// </summary>
    public class CompanyVerificationJobDao : EntityWithIdDao<Guid>
    {
        /// <summary>
        /// Идентификатор профиля компании
        /// </summary>
        public Guid CompanyProfileId { get; set; }

        /// <summary>
        /// ИНН компании для проверки
        /// </summary>
        [MaxLength(12)]
        public string Inn { get; set; } = string.Empty;

        /// <summary>
        /// Статус задания
        /// </summary>
        public short StatusId { get; set; }

        /// <summary>
        /// Комментарий к статусу
        /// </summary>
        [MaxLength(2000)]
        public string? StatusComment { get; set; }

        /// <summary>
        /// IP адрес, с которого создано задание
        /// </summary>
        [MaxLength(50)]
        public string IpAddress { get; set; } = string.Empty;

        /// <summary>
        /// Идентификатор результата верификации (после успешной проверки)
        /// </summary>
        public Guid? CompanyVerificationId { get; set; }

        /// <summary>
        /// Дата создания задания
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Дата последнего обновления
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Дата завершения задания
        /// </summary>
        public DateTime? CompletedAt { get; set; }

        /// <summary>
        /// Навигационное свойство к профилю компании
        /// </summary>
        public virtual CompanyProfileDao CompanyProfile { get; set; } = null!;

        /// <summary>
        /// Навигационное свойство к статусу
        /// </summary>
        public virtual CompanyVerificationJobStatusDao Status { get; set; } = null!;

        /// <summary>
        /// Навигационное свойство к результату верификации
        /// </summary>
        public virtual CompanyVerificationDao? CompanyVerification { get; set; }

        /// <summary>
        /// Навигационное свойство к истории изменений
        /// </summary>
        public virtual ICollection<CompanyVerificationJobHistoryDao> History { get; set; } = null!;
    }
}
