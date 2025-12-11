using Newtonsoft.Json;
using Renoza.Domain.Messages.Base;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Renoza.Domain.Messages.CashReceipt
{
    /// <summary>
    /// Сообщение для распознавания кассового чека через внешний API
    /// </summary>
    [DataContract]
    [Serializable]
    public class CashReceiptRecognitionMessage : IQueueMessage
    {
        /// <summary>
        /// Идентификатор задания
        /// </summary>
        [Display(Name = "Id задания")]
        [DataMember]
        [JsonProperty(PropertyName = "job_id")]
        public Guid JobId { get; set; }

        /// <summary>
        /// Фискальный номер
        /// </summary>
        [Display(Name = "Фискальный номер")]
        [DataMember]
        [JsonProperty(PropertyName = "fiscal_number")]
        public string FiscalNumber { get; set; } = string.Empty;

        /// <summary>
        /// Фискальный документ
        /// </summary>
        [Display(Name = "Фискальный документ")]
        [DataMember]
        [JsonProperty(PropertyName = "fiscal_document")]
        public string FiscalDocument { get; set; } = string.Empty;

        /// <summary>
        /// Фискальный признак
        /// </summary>
        [Display(Name = "Фискальный признак")]
        [DataMember]
        [JsonProperty(PropertyName = "fiscal_sign")]
        public string FiscalSign { get; set; } = string.Empty;

        /// <summary>
        /// n - Тип операции, соответствующей фискальному документу.
        /// "1" — приход средств (кассовый чек (БСО), кассовый чек коррекции (БСО коррекции);
        /// "2" — возврат прихода(кассовый чек (БСО));
        /// "3" — расход средств(кассовый чек (БСО), кассовый чек коррекции (БСО коррекции);
        /// "4" — возврат расхода(кассовый чек (БСО)).
        /// </summary>
        [Display(Name = "Тип операции")]
        [DataMember]
        [JsonProperty(PropertyName = "receipt_operation_type")]
        public string ReceiptOperationType { get; set; }

        /// <summary>
        /// Дата и время покупки
        /// </summary>
        [Display(Name = "Дата и время покупки")]
        [DataMember]
        [JsonProperty(PropertyName = "purchase_datetime")]
        public DateTime PurchaseDateTime { get; set; }

        /// <summary>
        /// Сумма покупки
        /// </summary>
        [Display(Name = "Сумма покупки")]
        [DataMember]
        [JsonProperty(PropertyName = "total_sum")]
        public decimal TotalSum { get; set; }
    }
}
