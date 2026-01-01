using Newtonsoft.Json;
using Renoza.Common.Base.Entities;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Renoza.Domain.Entities.CompanyProfiles
{
    /// <summary>
    /// Профиль компании (юридического лица)
    /// </summary>
    [DataContract]
    [Serializable]
    public class CompanyProfile : IEntityWithId<Guid>
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [Display(Name = "Id")]
        [DataMember]
        [JsonProperty(PropertyName = "Id")]
        public Guid Id { get; set; }

        /// <summary>
        /// ИНН компании
        /// </summary>
        [Display(Name = "ИНН компании")]
        [DataMember]
        [JsonProperty(PropertyName = "Inn")]
        public string Inn { get; set; } = string.Empty;

        /// <summary>
        /// ID типа компании (1 - юридическое лицо, 2 - индивидуальный предприниматель, 0 - неизвестно)
        /// </summary>
        [Display(Name = "Тип компании")]
        [DataMember]
        [JsonProperty(PropertyName = "CompanyTypeId")]
        public short CompanyTypeId { get; set; } = 0;

        /// <summary>
        /// Идентификатор результата верификации
        /// </summary>
        [Display(Name = "Идентификатор верификации")]
        [DataMember]
        [JsonProperty(PropertyName = "CompanyVerificationId")]
        public Guid? CompanyVerificationId { get; set; }

        /// <summary>
        /// Признак верификации компании
        /// </summary>
        [Display(Name = "Признак верификации")]
        [DataMember]
        [JsonProperty(PropertyName = "IsCompanyVerified")]
        public bool IsCompanyVerified { get; set; }

        /// <summary>
        /// Дата верификации компании
        /// </summary>
        [Display(Name = "Дата верификации")]
        [DataMember]
        [JsonProperty(PropertyName = "CompanyVerifiedAt")]
        public DateTime? CompanyVerifiedAt { get; set; }

        /// <summary>
        /// Полное наименование с ОПФ (например, "ООО \"ЮЗТЕХ\"" или "ИП Матлашов Петр Егорович")
        /// </summary>
        [Display(Name = "Полное наименование")]
        [DataMember]
        [JsonProperty(PropertyName = "FullName")]
        public string? FullName { get; set; }

        /// <summary>
        /// Краткое наименование (для юр.лиц, например "ЮЗТЕХ") или null для ИП
        /// </summary>
        [Display(Name = "Краткое наименование")]
        [DataMember]
        [JsonProperty(PropertyName = "ShortName")]
        public string? ShortName { get; set; }

        /// <summary>
        /// ОГРН
        /// </summary>
        [Display(Name = "ОГРН")]
        [DataMember]
        [JsonProperty(PropertyName = "Ogrn")]
        public string? Ogrn { get; set; }

        /// <summary>
        /// КПП (только для юридических лиц, null для ИП)
        /// </summary>
        [Display(Name = "КПП")]
        [DataMember]
        [JsonProperty(PropertyName = "Kpp")]
        public string? Kpp { get; set; }

        /// <summary>
        /// Адрес (полный адрес одной строкой)
        /// </summary>
        [Display(Name = "Адрес")]
        [DataMember]
        [JsonProperty(PropertyName = "Address")]
        public string? Address { get; set; }

        /// <summary>
        /// ФИО руководителя (для юр.лиц) или ФИО предпринимателя (для ИП)
        /// </summary>
        [Display(Name = "Руководитель/ИП")]
        [DataMember]
        [JsonProperty(PropertyName = "DirectorName")]
        public string? DirectorName { get; set; }

        /// <summary>
        /// Дата регистрации в ЕГРЮЛ/ЕГРИП
        /// </summary>
        [Display(Name = "Дата регистрации")]
        [DataMember]
        [JsonProperty(PropertyName = "RegistrationDate")]
        public DateOnly? RegistrationDate { get; set; }

        /// <summary>
        /// Статус организации (например, "ACTIVE", "LIQUIDATING", "LIQUIDATED")
        /// </summary>
        [Display(Name = "Статус")]
        [DataMember]
        [JsonProperty(PropertyName = "Status")]
        public string? Status { get; set; }

        /// <summary>
        /// Дата создания профиля
        /// </summary>
        [Display(Name = "Дата создания")]
        [DataMember]
        [JsonProperty(PropertyName = "CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Дата последнего обновления
        /// </summary>
        [Display(Name = "Дата обновления")]
        [DataMember]
        [JsonProperty(PropertyName = "UpdatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
