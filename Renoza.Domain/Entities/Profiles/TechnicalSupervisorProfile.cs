using Newtonsoft.Json;
using Renoza.Common.Base.Entities;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Renoza.Domain.Entities.Profiles
{
    /// <summary>
    /// Профиль технического надзора
    /// </summary>
    [DataContract]
    [Serializable]
    public class TechnicalSupervisorProfile : IEntityWithId<Guid>
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
        /// Идентификатор профиля компании
        /// </summary>
        [Display(Name = "Идентификатор профиля компании")]
        [DataMember]
        [JsonProperty(PropertyName = "CompanyProfileId")]
        public Guid CompanyProfileId { get; set; }

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
