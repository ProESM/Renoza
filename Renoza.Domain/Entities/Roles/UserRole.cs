using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Renoza.Domain.Entities.Roles
{
    /// <summary>
    /// Связь пользователя с ролью
    /// </summary>
    [DataContract]
    [Serializable]
    public class UserRole
    {
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        [Display(Name = "Идентификатор пользователя")]
        [DataMember]
        [JsonProperty(PropertyName = "UserId")]
        public Guid UserId { get; set; }

        /// <summary>
        /// Идентификатор роли
        /// </summary>
        [Display(Name = "Идентификатор роли")]
        [DataMember]
        [JsonProperty(PropertyName = "RoleId")]
        public Guid RoleId { get; set; }

        /// <summary>
        /// Признак активности роли для пользователя
        /// </summary>
        [Display(Name = "Признак активности")]
        [DataMember]
        [JsonProperty(PropertyName = "IsActive")]
        public bool IsActive { get; set; }

        /// <summary>
        /// Дата и время создания связи
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
