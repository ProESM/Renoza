using Renoza.Infrastructure.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace Renoza.Infrastructure.Entities.Renoza
{
    /// <summary>
    /// Верификация электронной почты
    /// </summary>
    public class EmailVerificationDao : EntityWithIdDao<Guid>
    {
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Адрес электронной почты
        /// </summary>
        [MaxLength(256)]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Код верификации
        /// </summary>
        [MaxLength(10)]
        public string VerificationCode { get; set; } = string.Empty;

        /// <summary>
        /// Признак верификации
        /// </summary>
        public bool IsVerified { get; set; }

        /// <summary>
        /// Дата и время истечения кода
        /// </summary>
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// Дата и время верификации
        /// </summary>
        public DateTime? VerifiedAt { get; set; }

        /// <summary>
        /// Дата и время создания
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Дата и время обновления
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Навигационное свойство к пользователю
        /// </summary>
        public virtual UserDao? User { get; set; }
    }
}
