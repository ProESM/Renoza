using Newtonsoft.Json;
using Renoza.Common.Base.Entities;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Renoza.Domain.Entities.CompanyMembers
{
    /// <summary>
    /// Участник компании (связь пользователя с компанией)
    /// </summary>
    [DataContract]
    [Serializable]
    public class CompanyMember : IEntityWithId<Guid>
    {
        /// <summary>
        /// Идентификатор участника компании
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
        /// Идентификатор роли участника в компании
        /// </summary>
        [Display(Name = "Идентификатор роли")]
        [DataMember]
        [JsonProperty(PropertyName = "MemberRoleId")]
        public Guid MemberRoleId { get; set; }

        /// <summary>
        /// Должность в компании
        /// </summary>
        [Display(Name = "Должность")]
        [DataMember]
        [JsonProperty(PropertyName = "Position")]
        public string? Position { get; set; }

        /// <summary>
        /// Дата вступления в компанию
        /// </summary>
        [Display(Name = "Дата вступления")]
        [DataMember]
        [JsonProperty(PropertyName = "JoinedAt")]
        public DateTime JoinedAt { get; set; }

        /// <summary>
        /// Дата выхода из компании
        /// </summary>
        [Display(Name = "Дата выхода")]
        [DataMember]
        [JsonProperty(PropertyName = "LeftAt")]
        public DateTime? LeftAt { get; set; }

        /// <summary>
        /// Признак активности участника в компании
        /// </summary>
        [Display(Name = "Признак активности")]
        [DataMember]
        [JsonProperty(PropertyName = "IsActive")]
        public bool IsActive { get; set; }

        /// <summary>
        /// Дата и время создания записи
        /// </summary>
        [Display(Name = "Дата и время создания")]
        [DataMember]
        [JsonProperty(PropertyName = "CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Дата и время редактирования записи
        /// </summary>
        [Display(Name = "Дата и время редактирования")]
        [DataMember]
        [JsonProperty(PropertyName = "UpdatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
