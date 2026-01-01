using Newtonsoft.Json;
using Renoza.Common.Base.Entities;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Renoza.Domain.Entities.CompanyVerification
{
    /// <summary>
    /// История изменений статусов задания на верификацию компании
    /// </summary>
    [DataContract]
    [Serializable]
    public class CompanyVerificationJobHistory : IEntityWithId<long>
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [Display(Name = "Id")]
        [DataMember]
        [JsonProperty(PropertyName = "Id")]
        public long Id { get; set; }

        /// <summary>
        /// Идентификатор задания
        /// </summary>
        [Display(Name = "Идентификатор задания")]
        [DataMember]
        [JsonProperty(PropertyName = "JobId")]
        public Guid JobId { get; set; }

        /// <summary>
        /// Идентификатор статуса
        /// </summary>
        [Display(Name = "Идентификатор статуса")]
        [DataMember]
        [JsonProperty(PropertyName = "StatusId")]
        public short StatusId { get; set; }

        /// <summary>
        /// Комментарий к изменению статуса
        /// </summary>
        [Display(Name = "Комментарий")]
        [DataMember]
        [JsonProperty(PropertyName = "Comment")]
        public string? Comment { get; set; }

        /// <summary>
        /// Дата создания записи истории
        /// </summary>
        [Display(Name = "Дата создания")]
        [DataMember]
        [JsonProperty(PropertyName = "CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
