using Newtonsoft.Json;
using Renoza.Common.Base.Entities;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Renoza.Domain.Entities.Countries
{
    /// <summary>
    /// Международный телефонный код
    /// </summary>
    [DataContract]
    [Serializable]
    public class PhoneCountryCode : IEntityWithId<int>
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [Display(Name = "Id")]
        [DataMember]
        [JsonProperty(PropertyName = "Id")]
        public int Id { get; set; }
        /// <summary>
        /// Телефонный код (например: +7, +1, +44)
        /// </summary>
        [Display(Name = "Код")]
        [DataMember]
        [JsonProperty(PropertyName = "Code")]
        public string Code { get; set; } = string.Empty;
        /// <summary>
        /// Формат телефонного номера (например: (XXX) XXX-XX-XX)
        /// </summary>
        [Display(Name = "Формат")]
        [DataMember]
        [JsonProperty(PropertyName = "PhoneFormat")]
        public string PhoneFormat { get; set; } = string.Empty;
        /// <summary>
        /// Идентификатор страны
        /// </summary>
        [Display(Name = "Идентификатор страны")]
        [DataMember]
        [JsonProperty(PropertyName = "CountryId")]
        public int CountryId { get; set; }
        /// <summary>
        /// Страна
        /// </summary>
        [Display(Name = "Страна")]
        [DataMember]
        [JsonProperty(PropertyName = "Country")]
        public Country Country { get; set; } = null!;
    }
}
