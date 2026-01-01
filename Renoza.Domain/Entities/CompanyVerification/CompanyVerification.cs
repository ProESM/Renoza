using Newtonsoft.Json;
using Renoza.Common.Base.Entities;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Renoza.Domain.Entities.CompanyVerification
{
    /// <summary>
    /// Результат верификации компании
    /// </summary>
    [DataContract]
    [Serializable]
    public class CompanyVerification : IEntityWithId<Guid>
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [Display(Name = "Id")]
        [DataMember]
        [JsonProperty(PropertyName = "Id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Идентификатор профиля компании
        /// </summary>
        [Display(Name = "Идентификатор профиля компании")]
        [DataMember]
        [JsonProperty(PropertyName = "CompanyProfileId")]
        public Guid CompanyProfileId { get; set; }

        /// <summary>
        /// ИНН компании
        /// </summary>
        [Display(Name = "ИНН")]
        [DataMember]
        [JsonProperty(PropertyName = "Inn")]
        public string Inn { get; set; } = string.Empty;

        /// <summary>
        /// КПП компании
        /// </summary>
        [Display(Name = "КПП")]
        [DataMember]
        [JsonProperty(PropertyName = "Kpp")]
        public string? Kpp { get; set; }

        /// <summary>
        /// ОГРН компании
        /// </summary>
        [Display(Name = "ОГРН")]
        [DataMember]
        [JsonProperty(PropertyName = "Ogrn")]
        public string? Ogrn { get; set; }

        /// <summary>
        /// Полное наименование компании
        /// </summary>
        [Display(Name = "Полное наименование")]
        [DataMember]
        [JsonProperty(PropertyName = "FullName")]
        public string? FullName { get; set; }

        /// <summary>
        /// Краткое наименование компании
        /// </summary>
        [Display(Name = "Краткое наименование")]
        [DataMember]
        [JsonProperty(PropertyName = "ShortName")]
        public string? ShortName { get; set; }

        /// <summary>
        /// Юридический адрес
        /// </summary>
        [Display(Name = "Юридический адрес")]
        [DataMember]
        [JsonProperty(PropertyName = "LegalAddress")]
        public string? LegalAddress { get; set; }

        /// <summary>
        /// Фактический адрес
        /// </summary>
        [Display(Name = "Фактический адрес")]
        [DataMember]
        [JsonProperty(PropertyName = "ActualAddress")]
        public string? ActualAddress { get; set; }

        /// <summary>
        /// ФИО руководителя
        /// </summary>
        [Display(Name = "ФИО руководителя")]
        [DataMember]
        [JsonProperty(PropertyName = "DirectorName")]
        public string? DirectorName { get; set; }

        /// <summary>
        /// Должность руководителя
        /// </summary>
        [Display(Name = "Должность руководителя")]
        [DataMember]
        [JsonProperty(PropertyName = "DirectorPost")]
        public string? DirectorPost { get; set; }

        /// <summary>
        /// Дата регистрации компании
        /// </summary>
        [Display(Name = "Дата регистрации")]
        [DataMember]
        [JsonProperty(PropertyName = "RegistrationDate")]
        public DateOnly? RegistrationDate { get; set; }

        /// <summary>
        /// Статус компании (активна/ликвидирована)
        /// </summary>
        [Display(Name = "Статус компании")]
        [DataMember]
        [JsonProperty(PropertyName = "CompanyStatus")]
        public string? CompanyStatus { get; set; }

        /// <summary>
        /// Полный JSON ответ от внешнего сервиса
        /// </summary>
        [Display(Name = "Ответ от сервиса")]
        [DataMember]
        [JsonProperty(PropertyName = "ExternalServiceResponse")]
        public string? ExternalServiceResponse { get; set; }

        /// <summary>
        /// Дата верификации
        /// </summary>
        [Display(Name = "Дата верификации")]
        [DataMember]
        [JsonProperty(PropertyName = "CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
