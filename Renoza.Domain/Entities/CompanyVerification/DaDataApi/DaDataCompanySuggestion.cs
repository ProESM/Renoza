using Newtonsoft.Json;

namespace Renoza.Domain.Entities.CompanyVerification.DaDataApi
{
    /// <summary>
    /// Подсказка о компании от DaData API
    /// </summary>
    public class DaDataCompanySuggestion
    {
        /// <summary>
        /// Текстовое представление
        /// </summary>
        [JsonProperty("value")]
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// Код нереликвизованности (0-1)
        /// </summary>
        [JsonProperty("unrestricted_value")]
        public string UnrestrictedValue { get; set; } = string.Empty;

        /// <summary>
        /// Данные о компании
        /// </summary>
        [JsonProperty("data")]
        public DaDataCompanyData Data { get; set; } = new();
    }
}
