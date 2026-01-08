using Renoza.Infrastructure.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace Renoza.Infrastructure.Entities.Renoza
{
    /// <summary>
    /// Запрос на вступление в компанию
    /// </summary>
    public class CompanyJoinRequestDao : EntityWithIdDao<Guid>
    {
        /// <summary>
        /// Идентификатор пользователя, который запрашивает вступление
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Идентификатор профиля компании
        /// </summary>
        public Guid CompanyProfileId { get; set; }

        /// <summary>
        /// Статус запроса (Pending, Approved, Rejected, Cancelled)
        /// </summary>
        public short StatusId { get; set; }

        /// <summary>
        /// Комментарий к запросу от пользователя
        /// </summary>
        [MaxLength(1000)]
        public string? RequestMessage { get; set; }

        /// <summary>
        /// Комментарий при одобрении/отклонении от владельца/менеджера
        /// </summary>
        [MaxLength(1000)]
        public string? ResponseMessage { get; set; }

        /// <summary>
        /// Дата создания запроса
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Дата обновления запроса
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// ID пользователя, который рассмотрел запрос (владелец/менеджер компании)
        /// </summary>
        public Guid? ReviewerId { get; set; }

        /// <summary>
        /// Дата рассмотрения запроса
        /// </summary>
        public DateTime? ReviewedAt { get; set; }

        /// <summary>
        /// Навигационное свойство к пользователю, который создал запрос
        /// </summary>
        public virtual UserDao User { get; set; } = null!;

        /// <summary>
        /// Навигационное свойство к профилю компании
        /// </summary>
        public virtual CompanyProfileDao CompanyProfile { get; set; } = null!;

        /// <summary>
        /// Навигационное свойство к статусу запроса
        /// </summary>
        public virtual CompanyJoinRequestStatusDao Status { get; set; } = null!;

        /// <summary>
        /// Навигационное свойство к пользователю, который рассмотрел запрос
        /// </summary>
        public virtual UserDao? Reviewer { get; set; }
    }
}
