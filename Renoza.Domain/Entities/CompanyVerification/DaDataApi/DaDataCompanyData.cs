using Newtonsoft.Json;

namespace Renoza.Domain.Entities.CompanyVerification.DaDataApi
{
    /// <summary>
    /// Данные о компании от DaData API
    /// </summary>
    public class DaDataCompanyData
    {
        /// <summary>
        /// ИНН
        /// </summary>
        [JsonProperty("inn")]
        public string? Inn { get; set; }

        /// <summary>
        /// КПП
        /// </summary>
        [JsonProperty("kpp")]
        public string? Kpp { get; set; }

        /// <summary>
        /// ОГРН
        /// </summary>
        [JsonProperty("ogrn")]
        public string? Ogrn { get; set; }

        /// <summary>
        /// Тип организации (LEGAL - юридическое лицо, INDIVIDUAL - индивидуальный предприниматель)
        /// </summary>
        [JsonProperty("type")]
        public string? Type { get; set; }

        /// <summary>
        /// Полное наименование с ОПФ
        /// </summary>
        [JsonProperty("name")]
        public DaDataCompanyName? Name { get; set; }

        /// <summary>
        /// Адрес
        /// </summary>
        [JsonProperty("address")]
        public DaDataCompanyAddress? Address { get; set; }

        /// <summary>
        /// Руководитель
        /// </summary>
        [JsonProperty("management")]
        public DaDataManagement? Management { get; set; }

        /// <summary>
        /// Дата регистрации
        /// </summary>
        [JsonProperty("ogrn_date")]
        public long? OgrnDate { get; set; }

        /// <summary>
        /// Статус организации
        /// </summary>
        [JsonProperty("state")]
        public DaDataCompanyState? State { get; set; }

        /// <summary>
        /// ФИО индивидуального предпринимателя (только для INDIVIDUAL)
        /// </summary>
        [JsonProperty("fio")]
        public DaDataFio? Fio { get; set; }

        /// <summary>
        /// Гражданство (только для INDIVIDUAL)
        /// </summary>
        [JsonProperty("citizenship")]
        public string? Citizenship { get; set; }

        /// <summary>
        /// Тип подразделения (только для LEGAL): MAIN - головная организация, BRANCH - филиал
        /// </summary>
        [JsonProperty("branch_type")]
        public string? BranchType { get; set; }

        /// <summary>
        /// Количество филиалов (только для LEGAL)
        /// </summary>
        [JsonProperty("branch_count")]
        public int? BranchCount { get; set; }
    }
}
