using Newtonsoft.Json;
using Renoza.Domain.Enums;
using Renoza.Domain.Messages.Base;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Renoza.Domain.Messages.CashReceipt
{
    /// <summary>
    /// Сообщение для валидации кассового чека
    /// </summary>
    [DataContract]
    [Serializable]
    public class CashReceiptValidationMessage : IQueueMessage
    {
        /// <summary>
        /// Идентификатор задания
        /// </summary>
        [Display(Name = "Id задания")]
        [DataMember]
        [JsonProperty(PropertyName = "job_id")]
        public Guid JobId { get; set; }

        /// <summary>
        /// Тип входных данных чека
        /// </summary>
        [Display(Name = "Тип входных данных")]
        [DataMember]
        [JsonProperty(PropertyName = "input_type")]
        public ReceiptInputType InputType { get; set; }

        /// <summary>
        /// Текст из QR кода для валидации
        /// </summary>
        [Display(Name = "QrSource")]
        [DataMember]
        [JsonProperty(PropertyName = "qr_source")]
        public string QrSource { get; set; } = string.Empty;
    }
}
