using Newtonsoft.Json;
using Renoza.Domain.Enums;
using Renoza.Domain.Messages.Base;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Renoza.Domain.Messages.CashReceipt
{
    /// <summary>
    /// Сообщение о входящем кассовом чеке
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
        [JsonProperty(PropertyName = "ip_address")]
        public string IpAddress { get; set; }

        /// <summary>
        /// Тип входных данных чека
        /// </summary>
        [Display(Name = "Тип входных данных")]
        [DataMember]
        [JsonProperty(PropertyName = "input_type")]
        public ReceiptInputType InputType { get; set; }

        /// <summary>
        /// Данные чека в строковом виде
        /// Для QrCode: текст из QR кода
        /// Для Photo/ImageFile/PdfFile: JSON сериализованный объект ReceiptData
        /// </summary>
        [Display(Name = "Данные чека")]
        [DataMember]
        [JsonProperty(PropertyName = "data")]
        public string Data { get; set; }

        /// <summary>
        /// Временный URL файла в S3 (папка temp/)
        /// Заполняется для InputType = Photo, ImageFile, PdfFile
        /// </summary>
        [Display(Name = "Временный URL файла в S3")]
        [DataMember]
        [JsonProperty(PropertyName = "file_temp_s3_url")]
        public string? FileTempS3Url { get; set; }

        /// <summary>
        /// Имя файла (для InputType = Photo, ImageFile, PdfFile)
        /// </summary>
        [Display(Name = "Имя файла")]
        [DataMember]
        [JsonProperty(PropertyName = "file_name")]
        public string? FileName { get; set; }

        /// <summary>
        /// Тип содержимого файла (для InputType = Photo, ImageFile, PdfFile)
        /// </summary>
        [Display(Name = "Тип файла")]
        [DataMember]
        [JsonProperty(PropertyName = "file_content_type")]
        public string? FileContentType { get; set; }
    }
}
