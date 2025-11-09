using Newtonsoft.Json;
using Renoza.Common.Base.Entities;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Renoza.Domain.Entities.Users
{
    /// <summary>
    /// Пользователь
    /// </summary>
    [DataContract]
    [Serializable]
    public class User : IEntityWithId<int>
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [Display(Name = "Id")]
        [DataMember]
        [JsonProperty(PropertyName = "Id")]
        public int Id { get; set; }
        /// <summary>
        /// Наименование
        /// </summary>
        [Display(Name = "Наименование")]
        [DataMember]
        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }
    }
}
