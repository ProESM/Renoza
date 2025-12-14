using Newtonsoft.Json;
using Renoza.Domain.Enums;
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
        /// Тип входных данных чека
        /// </summary>
        [Display(Name = "Тип входных данных")]
        [DataMember]
        [JsonProperty(PropertyName = "input_type")]
        public ReceiptInputType InputType { get; set; }

        /// <summary>
        /// JSON данные распознанного чека от внешнего API или введенные вручную
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
        /// URL файла чека в S3 (если используется существующий чек)
        /// </summary>
        [Display(Name = "URL файла")]
        [DataMember]
        [JsonProperty(PropertyName = "file_url")]
        public string? FileUrl { get; set; }

        /// <summary>
        /// Временный URL файла в S3 (папка temp/)
        /// Используется для перемещения файла в постоянное хранилище
        /// </summary>
        [Display(Name = "Временный URL файла в S3")]
        [DataMember]
        [JsonProperty(PropertyName = "file_temp_s3_url")]
        public string? FileTempS3Url { get; set; }

        /// <summary>
        /// Имя файла (для сохранения в постоянное хранилище)
        /// </summary>
        [Display(Name = "Имя файла")]
        [DataMember]
        [JsonProperty(PropertyName = "file_name")]
        public string? FileName { get; set; }

        /// <summary>
        /// Тип содержимого файла
        /// </summary>
        [Display(Name = "Тип файла")]
        [DataMember]
        [JsonProperty(PropertyName = "file_content_type")]
        public string? FileContentType { get; set; }
    }
}
