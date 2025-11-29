using Renoza.Infrastructure.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace Renoza.Infrastructure.Entities.Renoza
{
    /// <summary>
    /// Пользователь
    /// </summary>
    public class UserDao : EntityWithIdDao<Guid>
    {
        /// <summary>
        /// Имя пользователя
        /// </summary>
        [MaxLength(256)]
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Отображаемое имя
        /// </summary>
        [MaxLength(256)]
        public string DisplayName { get; set; } = string.Empty;
        /// <summary>
        /// Адрес электронной почты
        /// </summary>
        [MaxLength(256)]
        public string Email { get; set; } = string.Empty;
        /// <summary>
        /// Признак подтверждения электронной почты
        /// </summary>
        public bool IsEmailVerified { get; set; }
        /// <summary>
        /// Номер телефона
        /// </summary>
        [MaxLength(50)]
        public string PhoneNumber { get; set; } = string.Empty;
        /// <summary>
        /// Международный телефонный код
        /// </summary>
        [MaxLength(6)]
        public string PhoneCountryCode { get; set; } = string.Empty;
        /// <summary>
        /// Признак подтверждения номера телефона
        /// </summary>
        public bool IsPhoneNumberVerified { get; set; }
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
        /// Навигационное свойство: связи с паролями
        /// </summary>
        public virtual ICollection<UserPasswordDao> UserPasswords { get; set; } = null!;
        /// <summary>
        /// Навигационное свойство: связи с историей паролей
        /// </summary>
        public virtual ICollection<UserPasswordHistoryDao> UserPasswordHistory { get; set; } = null!;
        /// <summary>
        /// Навигационное свойство: связи с ролями
        /// </summary>
        public virtual ICollection<UserRoleDao> UserRoles { get; set; } = null!;
        /// <summary>
        /// Навигационное свойство: связи с профилями заказчиков
        /// </summary>
        public virtual ICollection<CustomerProfileDao> CustomerProfiles { get; set; } = null!;
        /// <summary>
        /// Навигационное свойство: связи с профилями работников
        /// </summary>
        public virtual ICollection<WorkerProfileDao> WorkerProfiles { get; set; } = null!;
        /// <summary>
        /// Навигационное свойство: связи с верификациями email
        /// </summary>
        public virtual ICollection<EmailVerificationDao> EmailVerifications { get; set; } = null!;
        /// <summary>
        /// Навигационное свойство: связи с верификациями телефона
        /// </summary>
        public virtual ICollection<PhoneVerificationDao> PhoneVerifications { get; set; } = null!;
    }
}
