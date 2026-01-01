using Newtonsoft.Json;
using Renoza.Common.Base.Entities;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Renoza.Domain.Entities.CompanyVerification
{
    /// <summary>
    /// Задание на верификацию компании
    /// </summary>
    [DataContract]
    [Serializable]
    public class CompanyVerificationJob : IEntityWithId<Guid>
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
        /// ИНН компании для проверки
        /// </summary>
        [Display(Name = "ИНН")]
        [DataMember]
        [JsonProperty(PropertyName = "Inn")]
        public string Inn { get; set; } = string.Empty;

        /// <summary>
        /// Статус задания
        /// </summary>
        [Display(Name = "Статус")]
        [DataMember]
        [JsonProperty(PropertyName = "StatusId")]
        public short StatusId { get; set; }

        /// <summary>
        /// Комментарий к статусу
        /// </summary>
        [Display(Name = "Комментарий")]
        [DataMember]
        [JsonProperty(PropertyName = "StatusComment")]
        public string? StatusComment { get; set; }

        /// <summary>
        /// IP адрес, с которого создано задание
        /// </summary>
        [Display(Name = "IP адрес")]
        [DataMember]
        [JsonProperty(PropertyName = "IpAddress")]
        public string IpAddress { get; set; } = string.Empty;

        /// <summary>
        /// Идентификатор результата верификации (после успешной проверки)
        /// </summary>
        [Display(Name = "Идентификатор верификации")]
        [DataMember]
        [JsonProperty(PropertyName = "CompanyVerificationId")]
        public Guid? CompanyVerificationId { get; set; }

        /// <summary>
        /// Дата создания задания
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

        /// <summary>
        /// Дата завершения задания
        /// </summary>
        [Display(Name = "Дата завершения")]
        [DataMember]
        [JsonProperty(PropertyName = "CompletedAt")]
        public DateTime? CompletedAt { get; set; }
    }
}
