using Newtonsoft.Json;

namespace Renoza.Domain.Entities.CompanyVerification.DaDataApi
{
    /// <summary>
    /// Наименования компании
    /// </summary>
    public class DaDataCompanyName
    {
        /// <summary>
        /// Полное наименование с ОПФ
        /// </summary>
        [JsonProperty("full_with_opf")]
        public string? FullWithOpf { get; set; }

        /// <summary>
        /// Краткое наименование с ОПФ
        /// </summary>
        [JsonProperty("short_with_opf")]
        public string? ShortWithOpf { get; set; }

        /// <summary>
        /// Полное наименование
        /// </summary>
        [JsonProperty("full")]
        public string? Full { get; set; }

        /// <summary>
        /// Краткое наименование
        /// </summary>
        [JsonProperty("short")]
        public string? Short { get; set; }
    }
}
