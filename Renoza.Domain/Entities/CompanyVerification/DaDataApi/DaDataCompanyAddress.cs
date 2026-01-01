using Newtonsoft.Json;

namespace Renoza.Domain.Entities.CompanyVerification.DaDataApi
{
    /// <summary>
    /// Адрес компании
    /// </summary>
    public class DaDataCompanyAddress
    {
        /// <summary>
        /// Адрес одной строкой
        /// </summary>
        [JsonProperty("value")]
        public string? Value { get; set; }

        /// <summary>
        /// Нереликвизированный адрес
        /// </summary>
        [JsonProperty("unrestricted_value")]
        public string? UnrestrictedValue { get; set; }

        /// <summary>
        /// Детали адреса
        /// </summary>
        [JsonProperty("data")]
        public DaDataAddressData? Data { get; set; }
    }
}
