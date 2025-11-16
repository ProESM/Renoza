using Newtonsoft.Json;
using Renoza.Common.Base.Entities;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Renoza.Domain.Entities.Profiles
{
    /// <summary>
    /// Профиль заказчика
    /// </summary>
    [DataContract]
    [Serializable]
    public class CustomerProfile : IEntityWithId<Guid>
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [Display(Name = "Id")]
        [DataMember]
        [JsonProperty(PropertyName = "Id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        [Display(Name = "Идентификатор пользователя")]
        [DataMember]
        [JsonProperty(PropertyName = "UserId")]
        public Guid UserId { get; set; }

        /// <summary>
        /// Название компании
        /// </summary>
        [Display(Name = "Название компании")]
        [DataMember]
        [JsonProperty(PropertyName = "CompanyName")]
        public string? CompanyName { get; set; }

        /// <summary>
        /// ИНН
        /// </summary>
        [Display(Name = "ИНН")]
        [DataMember]
        [JsonProperty(PropertyName = "TaxId")]
        public string? TaxId { get; set; }

        /// <summary>
        /// Адрес для выставления счетов
        /// </summary>
        [Display(Name = "Адрес для выставления счетов")]
        [DataMember]
        [JsonProperty(PropertyName = "BillingAddress")]
        public string? BillingAddress { get; set; }

        /// <summary>
        /// Кредитный лимит
        /// </summary>
        [Display(Name = "Кредитный лимит")]
        [DataMember]
        [JsonProperty(PropertyName = "CreditLimit")]
        public decimal? CreditLimit { get; set; }

        /// <summary>
        /// Признак активности профиля
        /// </summary>
        [Display(Name = "Признак активности")]
        [DataMember]
        [JsonProperty(PropertyName = "IsActive")]
        public bool IsActive { get; set; }

        /// <summary>
        /// Дата и время создания
        /// </summary>
        [Display(Name = "Дата и время создания")]
        [DataMember]
        [JsonProperty(PropertyName = "CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Дата и время редактирования
        /// </summary>
        [Display(Name = "Дата и время редактирования")]
        [DataMember]
        [JsonProperty(PropertyName = "UpdatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
