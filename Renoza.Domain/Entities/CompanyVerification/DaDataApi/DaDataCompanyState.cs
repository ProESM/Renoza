using Newtonsoft.Json;

namespace Renoza.Domain.Entities.CompanyVerification.DaDataApi
{
    /// <summary>
    /// Статус компании
    /// </summary>
    public class DaDataCompanyState
    {
        /// <summary>
        /// Статус организации (ACTIVE, LIQUIDATING, LIQUIDATED)
        /// </summary>
        [JsonProperty("status")]
        public string? Status { get; set; }

        /// <summary>
        /// Дата изменения статуса
        /// </summary>
        [JsonProperty("actuality_date")]
        public long? ActualityDate { get; set; }

        /// <summary>
        /// Дата регистрации в ПФР
        /// </summary>
        [JsonProperty("registration_date")]
        public long? RegistrationDate { get; set; }

        /// <summary>
        /// Дата ликвидации
        /// </summary>
        [JsonProperty("liquidation_date")]
        public long? LiquidationDate { get; set; }
    }
}
