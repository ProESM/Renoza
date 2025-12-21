using Newtonsoft.Json;
using Renoza.Common.Base.Entities;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Renoza.Domain.Entities.Documents
{
    /// <summary>
    /// Сгенерированный документ
    /// </summary>
    [DataContract]
    [Serializable]
    public class Document : IEntityWithId<Guid>
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [Display(Name = "Id")]
        [DataMember]
        [JsonProperty(PropertyName = "Id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Идентификатор шаблона документа
        /// </summary>
        [Display(Name = "Идентификатор шаблона")]
        [DataMember]
        [JsonProperty(PropertyName = "TemplateId")]
        public Guid TemplateId { get; set; }

        /// <summary>
        /// Название шаблона
        /// </summary>
        [Display(Name = "Название шаблона")]
        [DataMember]
        [JsonProperty(PropertyName = "TemplateName")]
        public string TemplateName { get; set; } = string.Empty;

        /// <summary>
        /// Идентификатор формата документа
        /// </summary>
        [Display(Name = "Идентификатор формата")]
        [DataMember]
        [JsonProperty(PropertyName = "FormatId")]
        public short FormatId { get; set; }

        /// <summary>
        /// Наименование формата
        /// </summary>
        [Display(Name = "Формат")]
        [DataMember]
        [JsonProperty(PropertyName = "FormatName")]
        public string FormatName { get; set; } = string.Empty;

        /// <summary>
        /// Расширение файла
        /// </summary>
        [Display(Name = "Расширение файла")]
        [DataMember]
        [JsonProperty(PropertyName = "FileExtension")]
        public string FileExtension { get; set; } = string.Empty;

        /// <summary>
        /// Идентификатор статуса документа
        /// </summary>
        [Display(Name = "Идентификатор статуса")]
        [DataMember]
        [JsonProperty(PropertyName = "StatusId")]
        public short StatusId { get; set; }

        /// <summary>
        /// Наименование статуса
        /// </summary>
        [Display(Name = "Статус")]
        [DataMember]
        [JsonProperty(PropertyName = "StatusName")]
        public string StatusName { get; set; } = string.Empty;

        /// <summary>
        /// Название документа
        /// </summary>
        [Display(Name = "Название")]
        [DataMember]
        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Ссылка на файл документа в S3
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
        /// Данные, использованные для генерации документа
        /// </summary>
        [Display(Name = "Данные плейсхолдеров")]
        [DataMember]
        [JsonProperty(PropertyName = "PlaceholderData")]
        public Dictionary<string, string>? PlaceholderData { get; set; }

        /// <summary>
        /// Идентификатор заказа
        /// </summary>
        [Display(Name = "Идентификатор заказа")]
        [DataMember]
        [JsonProperty(PropertyName = "OrderId")]
        public Guid? OrderId { get; set; }

        /// <summary>
        /// Идентификатор заказчика
        /// </summary>
        [Display(Name = "Идентификатор заказчика")]
        [DataMember]
        [JsonProperty(PropertyName = "CustomerId")]
        public Guid? CustomerId { get; set; }

        /// <summary>
        /// Идентификатор пользователя, создавшего документ
        /// </summary>
        [Display(Name = "Идентификатор создателя")]
        [DataMember]
        [JsonProperty(PropertyName = "CreatedBy")]
        public Guid CreatedBy { get; set; }

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

        /// <summary>
        /// Дата и время первого скачивания документа
        /// </summary>
        [Display(Name = "Дата первого скачивания")]
        [DataMember]
        [JsonProperty(PropertyName = "FirstDownloadedAt")]
        public DateTime? FirstDownloadedAt { get; set; }

        /// <summary>
        /// Дата и время последнего скачивания документа
        /// </summary>
        [Display(Name = "Дата последнего скачивания")]
        [DataMember]
        [JsonProperty(PropertyName = "LastDownloadedAt")]
        public DateTime? LastDownloadedAt { get; set; }

        /// <summary>
        /// Количество скачиваний
        /// </summary>
        [Display(Name = "Количество скачиваний")]
        [DataMember]
        [JsonProperty(PropertyName = "DownloadCount")]
        public int DownloadCount { get; set; }
    }
}
