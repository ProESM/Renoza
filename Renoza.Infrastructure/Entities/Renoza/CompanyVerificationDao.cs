using Renoza.Infrastructure.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace Renoza.Infrastructure.Entities.Renoza
{
    /// <summary>
    /// Результат верификации компании
    /// </summary>
    public class CompanyVerificationDao : EntityWithIdDao<Guid>
    {
        /// <summary>
        /// Идентификатор профиля компании
        /// </summary>
        public Guid CompanyProfileId { get; set; }

        /// <summary>
        /// ИНН компании
        /// </summary>
        [MaxLength(12)]
        public string Inn { get; set; } = string.Empty;

        /// <summary>
        /// КПП компании
        /// </summary>
        [MaxLength(9)]
        public string? Kpp { get; set; }

        /// <summary>
        /// ОГРН компании
        /// </summary>
        [MaxLength(15)]
        public string? Ogrn { get; set; }

        /// <summary>
        /// Полное наименование компании
        /// </summary>
        [MaxLength(500)]
        public string? FullName { get; set; }

        /// <summary>
        /// Краткое наименование компании
        /// </summary>
        [MaxLength(500)]
        public string? ShortName { get; set; }

        /// <summary>
        /// Юридический адрес
        /// </summary>
        [MaxLength(1000)]
        public string? LegalAddress { get; set; }

        /// <summary>
        /// Фактический адрес
        /// </summary>
        [MaxLength(1000)]
        public string? ActualAddress { get; set; }

        /// <summary>
        /// ФИО руководителя
        /// </summary>
        [MaxLength(500)]
        public string? DirectorName { get; set; }

        /// <summary>
        /// Должность руководителя
        /// </summary>
        [MaxLength(256)]
        public string? DirectorPost { get; set; }

        /// <summary>
        /// Дата регистрации компании
        /// </summary>
        public DateOnly? RegistrationDate { get; set; }

        /// <summary>
        /// Статус компании (активна/ликвидирована)
        /// </summary>
        [MaxLength(100)]
        public string? CompanyStatus { get; set; }

        /// <summary>
        /// Полный JSON ответ от внешнего сервиса
        /// </summary>
        public string? ExternalServiceResponse { get; set; }

        /// <summary>
        /// Дата верификации
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Навигационное свойство к профилю компании
        /// </summary>
        public virtual CompanyProfileDao CompanyProfile { get; set; } = null!;
    }
}
