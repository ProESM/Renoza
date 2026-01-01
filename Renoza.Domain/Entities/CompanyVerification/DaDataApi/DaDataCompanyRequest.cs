using Newtonsoft.Json;

namespace Renoza.Domain.Entities.CompanyVerification.DaDataApi
{
    /// <summary>
    /// Запрос к DaData API для получения информации о компании по ИНН
    /// </summary>
    public class DaDataCompanyRequest
    {
        /// <summary>
        /// ИНН компании для поиска
        /// </summary>
        [JsonProperty("query")]
        public string Query { get; set; } = string.Empty;

        /// <summary>
        /// Количество результатов (по умолчанию 1)
        /// </summary>
        [JsonProperty("count")]
        public int Count { get; set; } = 1;
    }
}
