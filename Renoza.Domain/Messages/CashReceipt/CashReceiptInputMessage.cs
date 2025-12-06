using Newtonsoft.Json;
using Renoza.Domain.Messages.Base;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Renoza.Domain.Messages.CashReceipt
{
    /// <summary>
    /// Сообщение о входящем QR кассового чека
    /// </summary>
    [DataContract]
    [Serializable]
    public class CashReceiptInputMessage : IQueueMessage
    {
        /// <summary>
        /// Идентификатор задания
        /// </summary>
        [Display(Name = "Id задания")]
        [DataMember]
        [JsonProperty(PropertyName = "job_id")]
        public Guid JobId { get; set; }
        /// <summary>
        /// IP адрес, с которого выполняется запрос
        /// </summary>
        [Display(Name = "IP")]
        [DataMember]
        [JsonProperty(PropertyName = "IpAddress")]
        public string IpAddress { get; set; }
        /// <summary>
        /// Текст из QR кода
        /// </summary>
        [Display(Name = "QrSource")]
        [DataMember]
        [JsonProperty(PropertyName = "QrSource")]
        public string QrSource { get; set; }
    }
}
