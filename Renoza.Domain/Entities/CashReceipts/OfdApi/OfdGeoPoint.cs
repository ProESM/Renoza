using Newtonsoft.Json;

namespace Renoza.Domain.Entities.CashReceipts.OfdApi
{
    /// <summary>
    /// Географические координаты
    /// </summary>
    public class OfdGeoPoint
    {
        /// <summary>
        /// Широта
        /// </summary>
        [JsonProperty("Latitude")]
        public decimal? Latitude { get; set; }

        /// <summary>
        /// Долгота
        /// </summary>
        [JsonProperty("Longitude")]
        public decimal? Longitude { get; set; }
    }
}
