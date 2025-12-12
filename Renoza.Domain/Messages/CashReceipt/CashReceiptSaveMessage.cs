using Newtonsoft.Json;
using Renoza.Domain.Messages.Base;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Renoza.Domain.Messages.CashReceipt
{
    /// <summary>
    /// Сообщение для сохранения распознанного кассового чека в БД
    /// </summary>
    [DataContract]
    [Serializable]
    public class CashReceiptSaveMessage : IQueueMessage
    {
        /// <summary>
        /// Идентификатор задания
        /// </summary>
        [Display(Name = "Id задания")]
        [DataMember]
        [JsonProperty(PropertyName = "job_id")]
        public Guid JobId { get; set; }

        /// <summary>
        /// JSON данные распознанного чека от внешнего API
        /// </summary>
        [Display(Name = "JSON данные чека")]
        [DataMember]
        [JsonProperty(PropertyName = "receipt_json")]
        public string ReceiptJson { get; set; } = string.Empty;

        /// <summary>
        /// Идентификатор существующего чека (если переиспользуем ранее обработанный)
        /// </summary>
        [Display(Name = "Id существующего чека")]
        [DataMember]
        [JsonProperty(PropertyName = "cash_receipt_id")]
        public Guid? CashReceiptId { get; set; }

        /// <summary>
        /// URL PDF файла чека в S3 (если используется существующий чек)
        /// </summary>
        [Display(Name = "URL PDF файла")]
        [DataMember]
        [JsonProperty(PropertyName = "pdf_url")]
        public string? PdfUrl { get; set; }
    }
}
