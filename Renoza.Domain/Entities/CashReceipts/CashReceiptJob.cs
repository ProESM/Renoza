using Newtonsoft.Json;
using Renoza.Common.Base.Entities;
using Renoza.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Renoza.Domain.Entities.CashReceipts
{
    /// <summary>
    /// Запрос на загрузку чека
    /// </summary>
    [DataContract]
    [Serializable]
    public class CashReceiptJob : IEntityWithId<Guid>
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [Display(Name = "Id")]
        [DataMember]
        [JsonProperty(PropertyName = "Id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Идентификатор клиента
        /// </summary>
        [Display(Name = "Идентификатор клиента")]
        [DataMember]
        [JsonProperty(PropertyName = "CustomerId")]
        public Guid CustomerId { get; set; }

        /// <summary>
        /// Идентификатор пользователя, создавшего запрос
        /// </summary>
        [Display(Name = "Идентификатор создателя")]
        [DataMember]
        [JsonProperty(PropertyName = "CreatedBy")]
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// Идентификатор заказа (опционально)
        /// </summary>
        [Display(Name = "Идентификатор заказа")]
        [DataMember]
        [JsonProperty(PropertyName = "OrderId")]
        public Guid? OrderId { get; set; }

        /// <summary>
        /// Идентификатор статуса
        /// </summary>
        [Display(Name = "Идентификатор статуса")]
        [DataMember]
        [JsonProperty(PropertyName = "StatusId")]
        public short StatusId { get; set; }

        /// <summary>
        /// Наименование статуса
        /// </summary>
        [Display(Name = "Наименование статуса")]
        [DataMember]
        [JsonProperty(PropertyName = "StatusName")]
        public string StatusName { get; set; } = string.Empty;

        /// <summary>
        /// Комментарий к текущему статусу
        /// </summary>
        [Display(Name = "Комментарий к статусу")]
        [DataMember]
        [JsonProperty(PropertyName = "StatusComment")]
        public string? StatusComment { get; set; }

        /// <summary>
        /// QR-код или источник данных
        /// </summary>
        [Display(Name = "QR-код или источник")]
        [DataMember]
        [JsonProperty(PropertyName = "QrSource")]
        public string QrSource { get; set; } = string.Empty;

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
        /// Дата и время завершения (опционально)
        /// </summary>
        [Display(Name = "Дата завершения")]
        [DataMember]
        [JsonProperty(PropertyName = "CompletedAt")]
        public DateTime? CompletedAt { get; set; }
    }
}
