using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Renoza.Domain.Entities.Profiles
{
    /// <summary>
    /// Профиль заказчика
    /// </summary>
    [DataContract]
    [Serializable]
    public class CustomerProfile : Profile
    {
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
    }
}
