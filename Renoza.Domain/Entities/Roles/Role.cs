using Newtonsoft.Json;
using Renoza.Common.Base.Entities;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Renoza.Domain.Entities.Roles
{
    /// <summary>
    /// Роль пользователя
    /// </summary>
    [DataContract]
    [Serializable]
    public class Role : IEntityWithId<Guid>
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [Display(Name = "Id")]
        [DataMember]
        [JsonProperty(PropertyName = "Id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Наименование роли
        /// </summary>
        [Display(Name = "Наименование")]
        [DataMember]
        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Описание роли
        /// </summary>
        [Display(Name = "Описание")]
        [DataMember]
        [JsonProperty(PropertyName = "Description")]
        public string? Description { get; set; }

        /// <summary>
        /// Признак системной роли (не может быть удалена)
        /// </summary>
        [Display(Name = "Системная роль")]
        [DataMember]
        [JsonProperty(PropertyName = "IsSystemRole")]
        public bool IsSystemRole { get; set; }

        /// <summary>
        /// Признак активности
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
