using Newtonsoft.Json;

namespace Renoza.Domain.Entities.CashReceipts.OfdApi
{
    /// <summary>
    /// Ошибка в ответе API OFD.ru
    /// </summary>
    public class OfdApiError
    {
        /// <summary>
        /// Внутренний код ошибки
        /// </summary>
        [JsonProperty("Code")]
        public string? Code { get; set; }

        /// <summary>
        /// Имя параметра, в котором возникла ошибка
        /// </summary>
        [JsonProperty("Field")]
        public string? Field { get; set; }

        /// <summary>
        /// Описание ошибки
        /// </summary>
        [JsonProperty("Message")]
        public string? Message { get; set; }
    }
}
