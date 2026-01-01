using Newtonsoft.Json;

namespace Renoza.Domain.Entities.CompanyVerification.DaDataApi
{
    /// <summary>
    /// Ответ от DaData API с информацией о компаниях
    /// </summary>
    public class DaDataCompanyResponse
    {
        /// <summary>
        /// Массив найденных компаний
        /// </summary>
        [JsonProperty("suggestions")]
        public List<DaDataCompanySuggestion>? Suggestions { get; set; } = new();
    }
}
