using Newtonsoft.Json;

namespace Renoza.Domain.Entities.CompanyVerification.DaDataApi
{
    /// <summary>
    /// Детали адреса
    /// </summary>
    public class DaDataAddressData
    {
        /// <summary>
        /// Почтовый индекс
        /// </summary>
        [JsonProperty("postal_code")]
        public string? PostalCode { get; set; }

        /// <summary>
        /// Страна
        /// </summary>
        [JsonProperty("country")]
        public string? Country { get; set; }

        /// <summary>
        /// Регион с типом
        /// </summary>
        [JsonProperty("region_with_type")]
        public string? RegionWithType { get; set; }

        /// <summary>
        /// Город с типом
        /// </summary>
        [JsonProperty("city_with_type")]
        public string? CityWithType { get; set; }

        /// <summary>
        /// Улица с типом
        /// </summary>
        [JsonProperty("street_with_type")]
        public string? StreetWithType { get; set; }

        /// <summary>
        /// Номер дома
        /// </summary>
        [JsonProperty("house")]
        public string? House { get; set; }

        /// <summary>
        /// Корпус/строение
        /// </summary>
        [JsonProperty("block")]
        public string? Block { get; set; }

        /// <summary>
        /// Квартира/офис
        /// </summary>
        [JsonProperty("flat")]
        public string? Flat { get; set; }
    }
}
