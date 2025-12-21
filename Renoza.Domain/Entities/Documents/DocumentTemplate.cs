using Newtonsoft.Json;
using Renoza.Common.Base.Entities;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Renoza.Domain.Entities.Documents
{
    /// <summary>
    /// Шаблон документа
    /// </summary>
    [DataContract]
    [Serializable]
    public class DocumentTemplate : IEntityWithId<Guid>
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [Display(Name = "Id")]
        [DataMember]
        [JsonProperty(PropertyName = "Id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Идентификатор типа шаблона
        /// </summary>
        [Display(Name = "Идентификатор типа")]
        [DataMember]
        [JsonProperty(PropertyName = "TemplateTypeId")]
        public short TemplateTypeId { get; set; }

        /// <summary>
        /// Наименование типа шаблона
        /// </summary>
        [Display(Name = "Тип шаблона")]
        [DataMember]
        [JsonProperty(PropertyName = "TemplateTypeName")]
        public string TemplateTypeName { get; set; } = string.Empty;

        /// <summary>
        /// Название шаблона
        /// </summary>
        [Display(Name = "Название")]
        [DataMember]
        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Описание шаблона
        /// </summary>
        [Display(Name = "Описание")]
        [DataMember]
        [JsonProperty(PropertyName = "Description")]
        public string? Description { get; set; }

        /// <summary>
        /// Ссылка на файл шаблона в S3
        /// </summary>
        [Display(Name = "Ссылка на файл")]
        [DataMember]
        [JsonProperty(PropertyName = "FileUrl")]
        public string FileUrl { get; set; } = string.Empty;

        /// <summary>
        /// Размер файла в байтах
        /// </summary>
        [Display(Name = "Размер файла")]
        [DataMember]
        [JsonProperty(PropertyName = "FileSize")]
        public long FileSize { get; set; }

        /// <summary>
        /// Список доступных плейсхолдеров
        /// </summary>
        [Display(Name = "Доступные плейсхолдеры")]
        [DataMember]
        [JsonProperty(PropertyName = "AvailablePlaceholders")]
        public List<string>? AvailablePlaceholders { get; set; }

        /// <summary>
        /// Идентификатор пользователя, создавшего шаблон
        /// </summary>
        [Display(Name = "Идентификатор создателя")]
        [DataMember]
        [JsonProperty(PropertyName = "CreatedBy")]
        public Guid CreatedBy { get; set; }

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

        /// <summary>
        /// Дата и время редактирования
        /// </summary>
        [Display(Name = "Дата обновления")]
        [DataMember]
        [JsonProperty(PropertyName = "UpdatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
