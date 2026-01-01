using Newtonsoft.Json;

namespace Renoza.Domain.Entities.CompanyVerification.DaDataApi
{
    /// <summary>
    /// Руководитель компании
    /// </summary>
    public class DaDataManagement
    {
        /// <summary>
        /// ФИО руководителя
        /// </summary>
        [JsonProperty("name")]
        public string? Name { get; set; }

        /// <summary>
        /// Должность руководителя
        /// </summary>
        [JsonProperty("post")]
        public string? Post { get; set; }

        /// <summary>
        /// Дата вступления в должность руководителя
        /// </summary>
        [JsonProperty("start_date")]
        public long? StartDate { get; set; }
    }
}
