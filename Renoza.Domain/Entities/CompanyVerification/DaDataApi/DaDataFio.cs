using Newtonsoft.Json;

namespace Renoza.Domain.Entities.CompanyVerification.DaDataApi
{
    /// <summary>
    /// ФИО индивидуального предпринимателя от DaData API
    /// </summary>
    public class DaDataFio
    {
        /// <summary>
        /// Фамилия
        /// </summary>
        [JsonProperty("surname")]
        public string? Surname { get; set; }

        /// <summary>
        /// Имя
        /// </summary>
        [JsonProperty("name")]
        public string? Name { get; set; }

        /// <summary>
        /// Отчество
        /// </summary>
        [JsonProperty("patronymic")]
        public string? Patronymic { get; set; }

        /// <summary>
        /// Пол
        /// </summary>
        [JsonProperty("gender")]
        public string? Gender { get; set; }

        /// <summary>
        /// Источник
        /// </summary>
        [JsonProperty("source")]
        public string? Source { get; set; }

        /// <summary>
        /// Код качества данных
        /// </summary>
        [JsonProperty("qc")]
        public string? Qc { get; set; }
    }
}
