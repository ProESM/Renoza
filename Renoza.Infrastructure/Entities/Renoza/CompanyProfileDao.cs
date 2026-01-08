using Renoza.Infrastructure.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace Renoza.Infrastructure.Entities.Renoza
{
    /// <summary>
    /// Профиль компании (общие данные для ремонтных бригад и технического надзора)
    /// </summary>
    public class CompanyProfileDao : EntityWithIdDao<Guid>
    {
        /// <summary>
        /// ИНН компании
        /// </summary>
        [MaxLength(12)]
        public string Inn { get; set; } = string.Empty;

        /// <summary>
        /// ID типа компании (FK к справочнику CompanyTypes)
        /// </summary>
        public short CompanyTypeId { get; set; } = 0;

        /// <summary>
        /// Идентификатор верификации компании (если успешно верифицирована).
        /// 
        /// ВАЖНО: На самом деле, связь должна быть только one-to-many:
        /// один CompanyProfile может иметь много CompanyVerification записей (история верификаций).
        /// Поле CompanyVerificationId в CompanyProfileDao нужно для быстрого
        /// доступа к актуальной верификации, но не должно быть навигационным
        /// свойством CompanyVerification.
        /// </summary>
        public Guid? CompanyVerificationId { get; set; }

        /// <summary>
        /// Признак успешной верификации компании
        /// </summary>
        public bool IsCompanyVerified { get; set; }

        /// <summary>
        /// Дата верификации компании
        /// </summary>
        public DateTime? CompanyVerifiedAt { get; set; }

        /// <summary>
        /// Полное наименование с ОПФ (например, "ООО \"ЮЗТЕХ\"" или "ИП Матлашов Петр Егорович")
        /// </summary>
        [MaxLength(512)]
        public string? FullName { get; set; }

        /// <summary>
        /// Краткое наименование (для юр.лиц, например "ЮЗТЕХ") или null для ИП
        /// </summary>
        [MaxLength(256)]
        public string? ShortName { get; set; }

        /// <summary>
        /// ОГРН
        /// </summary>
        [MaxLength(15)]
        public string? Ogrn { get; set; }

        /// <summary>
        /// КПП (только для юридических лиц, null для ИП)
        /// </summary>
        [MaxLength(9)]
        public string? Kpp { get; set; }

        /// <summary>
        /// Адрес (полный адрес одной строкой)
        /// </summary>
        [MaxLength(1024)]
        public string? Address { get; set; }

        /// <summary>
        /// ФИО руководителя (для юр.лиц) или ФИО предпринимателя (для ИП)
        /// </summary>
        [MaxLength(256)]
        public string? DirectorName { get; set; }

        /// <summary>
        /// Дата регистрации в ЕГРЮЛ/ЕГРИП
        /// </summary>
        public DateOnly? RegistrationDate { get; set; }

        /// <summary>
        /// Статус организации (например, "ACTIVE", "LIQUIDATING", "LIQUIDATED")
        /// </summary>
        [MaxLength(50)]
        public string? Status { get; set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Дата обновления
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Навигационное свойство к типу компании
        /// </summary>
        public virtual CompanyTypeDao CompanyType { get; set; } = null!;

        /// <summary>
        /// Навигационное свойство ко всем верификациям компании (история)
        /// </summary>
        public virtual ICollection<CompanyVerificationDao> CompanyVerifications { get; set; } = null!;

        /// <summary>
        /// Навигационное свойство к заданиям на верификацию
        /// </summary>
        public virtual ICollection<CompanyVerificationJobDao> CompanyVerificationJobs { get; set; } = null!;
    }
}
