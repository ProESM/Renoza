using Newtonsoft.Json;
using Renoza.Common.Base.Entities;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Renoza.Domain.Entities.Verifications
{
    /// <summary>
    /// Верификация электронной почты
    /// </summary>
    [DataContract]
    [Serializable]
    public class EmailVerification : IEntityWithId<Guid>
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
        /// Адрес электронной почты
        /// </summary>
        [Display(Name = "Email")]
        [DataMember]
        [JsonProperty(PropertyName = "Email")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Код верификации
        /// </summary>
        [Display(Name = "Код верификации")]
        [DataMember]
        [JsonProperty(PropertyName = "VerificationCode")]
        public string VerificationCode { get; set; } = string.Empty;

        /// <summary>
        /// Признак верификации
        /// </summary>
        [Display(Name = "Признак верификации")]
        [DataMember]
        [JsonProperty(PropertyName = "IsVerified")]
        public bool IsVerified { get; set; }

        /// <summary>
        /// Дата и время истечения кода
        /// </summary>
        [Display(Name = "Дата истечения")]
        [DataMember]
        [JsonProperty(PropertyName = "ExpiresAt")]
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// Дата и время верификации
        /// </summary>
        [Display(Name = "Дата верификации")]
        [DataMember]
        [JsonProperty(PropertyName = "VerifiedAt")]
        public DateTime? VerifiedAt { get; set; }

        /// <summary>
        /// Дата и время создания
        /// </summary>
        [Display(Name = "Дата создания")]
        [DataMember]
        [JsonProperty(PropertyName = "CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Дата и время обновления
        /// </summary>
        [Display(Name = "Дата обновления")]
        [DataMember]
        [JsonProperty(PropertyName = "UpdatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
