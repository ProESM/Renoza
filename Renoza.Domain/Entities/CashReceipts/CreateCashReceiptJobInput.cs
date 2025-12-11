using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Renoza.Domain.Entities.CashReceipts
{
    /// <summary>
    /// Входные данные для создания задания по загрузке чека
    /// </summary>
    [DataContract]
    [Serializable]
    public class CreateCashReceiptJobInput
    {
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
        /// QR-код или источник данных
        /// </summary>
        [Display(Name = "QR-код или источник данных")]
        [DataMember]
        [JsonProperty(PropertyName = "QrSource")]
        public string QrSource { get; set; } = string.Empty;

        /// <summary>
        /// IP адрес пользователя
        /// </summary>
        [Display(Name = "IP адрес")]
        [DataMember]
        [JsonProperty(PropertyName = "IpAddress")]
        public string IpAddress { get; set; } = string.Empty;

        /// <summary>
        /// Идентификатор заказа (опционально)
        /// </summary>
        [Display(Name = "Идентификатор заказа")]
        [DataMember]
        [JsonProperty(PropertyName = "OrderId")]
        public Guid? OrderId { get; set; }
    }
}
