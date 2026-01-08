using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Renoza.Domain.Entities.Profiles
{
    /// <summary>
    /// Профиль технического надзора
    /// </summary>
    [DataContract]
    [Serializable]
    public class TechnicalSupervisorProfile : Profile
    {
        /// <summary>
        /// Специализация (виды надзора)
        /// </summary>
        [Display(Name = "Специализация")]
        [DataMember]
        [JsonProperty(PropertyName = "Specialization")]
        public string? Specialization { get; set; }

        /// <summary>
        /// Сертификаты и квалификации
        /// </summary>
        [Display(Name = "Сертификаты")]
        [DataMember]
        [JsonProperty(PropertyName = "Certifications")]
        public string[]? Certifications { get; set; }

        /// <summary>
        /// Дата начала профессиональной деятельности
        /// </summary>
        [Display(Name = "Дата начала профессиональной деятельности")]
        [DataMember]
        [JsonProperty(PropertyName = "ProfessionalStartDate")]
        public DateOnly? ProfessionalStartDate { get; set; }

        /// <summary>
        /// Признак доступности для новых заказов
        /// </summary>
        [Display(Name = "Доступен для заказов")]
        [DataMember]
        [JsonProperty(PropertyName = "IsAvailable")]
        public bool IsAvailable { get; set; }

        /// <summary>
        /// Рейтинг
        /// </summary>
        [Display(Name = "Рейтинг")]
        [DataMember]
        [JsonProperty(PropertyName = "Rating")]
        public decimal? Rating { get; set; }
    }
}
