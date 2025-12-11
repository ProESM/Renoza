using Newtonsoft.Json;

namespace Renoza.Domain.Entities.CashReceipts.OfdApi
{
    /// <summary>
    /// Универсальный ответ от API OFD.ru (подходит как для успешных, так и для ошибочных ответов)
    /// </summary>
    public class OfdApiResponse
    {
        /// <summary>
        /// Успешность запроса
        /// </summary>
        [JsonProperty("Success")]
        public bool Success { get; set; }

        /// <summary>
        /// Данные чека (заполняется только при Success = true)
        /// </summary>
        [JsonProperty("Data")]
        public OfdReceiptData? Data { get; set; }

        /// <summary>
        /// Список ошибок (заполняется только при Success = false)
        /// </summary>
        [JsonProperty("Errors")]
        public List<OfdApiError>? Errors { get; set; }

        /// <summary>
        /// Доступное количество запросов по токену до конца текущего месяца
        /// </summary>
        [JsonProperty("AvailableRequests")]
        public int? AvailableRequests { get; set; }
    }
}
