using Newtonsoft.Json;
using Renoza.Common.Base.Entities;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Renoza.Domain.Entities.Documents
{
    /// <summary>
    /// Формат документа
    /// </summary>
    [DataContract]
    [Serializable]
    public class DocumentFormat : IEntityWithId<short>
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [Display(Name = "Id")]
        [DataMember]
        [JsonProperty(PropertyName = "Id")]
        public short Id { get; set; }

        /// <summary>
        /// Наименование формата
        /// </summary>
        [Display(Name = "Наименование")]
        [DataMember]
        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Расширение файла
        /// </summary>
        [Display(Name = "Расширение файла")]
        [DataMember]
        [JsonProperty(PropertyName = "FileExtension")]
        public string FileExtension { get; set; } = string.Empty;

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
        [Display(Name = "Дата создания")]
        [DataMember]
        [JsonProperty(PropertyName = "CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
