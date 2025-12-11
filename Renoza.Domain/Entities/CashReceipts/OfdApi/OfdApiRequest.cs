using Newtonsoft.Json;

namespace Renoza.Domain.Entities.CashReceipts.OfdApi
{
    /// <summary>
    /// Запрос к API OFD.ru для получения данных чека
    /// </summary>
    public class OfdApiRequest
    {
        /// <summary>
        /// Сумма чека в копейках
        /// </summary>
        [JsonProperty("TotalSum")]
        public int TotalSum { get; set; }

        /// <summary>
        /// Дата и время документа в формате ISO 8601 YYYY-MM-DDThh:mm:ss[.SSS]
        /// </summary>
        [JsonProperty("DocDateTime")]
        public string DocDateTime { get; set; } = string.Empty;

        /// <summary>
        /// Заводской номер фискального накопителя
        /// </summary>
        [JsonProperty("FnNumber")]
        public string FnNumber { get; set; } = string.Empty;

        /// <summary>
        /// Тип операции (1 - приход, 2 - возврат прихода, 3 - расход, 4 - возврат расхода)
        /// </summary>
        [JsonProperty("ReceiptOperationType")]
        public string ReceiptOperationType { get; set; } = "1";

        /// <summary>
        /// Номер фискального документа
        /// </summary>
        [JsonProperty("DocNumber")]
        public string DocNumber { get; set; } = string.Empty;

        /// <summary>
        /// Фискальный признак документа
        /// </summary>
        [JsonProperty("DocFiscalSign")]
        public string DocFiscalSign { get; set; } = string.Empty;

        /// <summary>
        /// Токен для доступа к API OFD.ru
        /// </summary>
        [JsonProperty("tokenSecret")]
        public string TokenSecret { get; set; } = string.Empty;
    }
}
