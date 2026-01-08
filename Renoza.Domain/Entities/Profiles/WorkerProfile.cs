using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Renoza.Domain.Entities.Profiles
{
    /// <summary>
    /// Профиль работника
    /// </summary>
    [DataContract]
    [Serializable]
    public class WorkerProfile : Profile
    {
        /// <summary>
        /// Специализация работника
        /// </summary>
        [Display(Name = "Специализация")]
        [DataMember]
        [JsonProperty(PropertyName = "Specialization")]
        public string? Specialization { get; set; }

        /// <summary>
        /// Размер команды
        /// </summary>
        [Display(Name = "Размер команды")]
        [DataMember]
        [JsonProperty(PropertyName = "TeamSize")]
        public int? TeamSize { get; set; }

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
        /// Рейтинг работника
        /// </summary>
        [Display(Name = "Рейтинг")]
        [DataMember]
        [JsonProperty(PropertyName = "Rating")]
        public decimal? Rating { get; set; }
    }
}
