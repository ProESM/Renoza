namespace Renoza.Domain.Entities.CompanyProfiles
{
    /// <summary>
    /// Данные для обновления профиля компании (дублированные поля из верификации)
    /// </summary>
    public class CompanyProfileUpdateData
    {
        /// <summary>
        /// Полное наименование с ОПФ (например, "ООО \"ЮЗТЕХ\"" или "ИП Матлашов Петр Егорович")
        /// </summary>
        public string? FullName { get; set; }

        /// <summary>
        /// Краткое наименование (для юр.лиц, например "ЮЗТЕХ") или null для ИП
        /// </summary>
        public string? ShortName { get; set; }

        /// <summary>
        /// ОГРН
        /// </summary>
        public string? Ogrn { get; set; }

        /// <summary>
        /// КПП (только для юридических лиц, null для ИП)
        /// </summary>
        public string? Kpp { get; set; }

        /// <summary>
        /// Адрес (полный адрес одной строкой)
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// ФИО руководителя (для юр.лиц) или ФИО предпринимателя (для ИП)
        /// </summary>
        public string? DirectorName { get; set; }

        /// <summary>
        /// Дата регистрации в ЕГРЮЛ/ЕГРИП
        /// </summary>
        public DateOnly? RegistrationDate { get; set; }

        /// <summary>
        /// Статус организации (например, "ACTIVE", "LIQUIDATING", "LIQUIDATED")
        /// </summary>
        public string? Status { get; set; }
    }
}
