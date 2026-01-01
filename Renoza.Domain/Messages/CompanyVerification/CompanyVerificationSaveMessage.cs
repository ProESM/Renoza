namespace Renoza.Domain.Messages.CompanyVerification
{
    /// <summary>
    /// Сообщение для сохранения результатов верификации компании
    /// </summary>
    public class CompanyVerificationSaveMessage
    {
        /// <summary>
        /// ID задания на верификацию
        /// </summary>
        public Guid JobId { get; set; }

        /// <summary>
        /// ID профиля компании
        /// </summary>
        public Guid CompanyProfileId { get; set; }

        /// <summary>
        /// ИНН компании
        /// </summary>
        public string Inn { get; set; } = string.Empty;

        /// <summary>
        /// Тип компании (LEGAL - юридическое лицо, INDIVIDUAL - индивидуальный предприниматель)
        /// </summary>
        public string? CompanyType { get; set; }

        /// <summary>
        /// Название компании
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Полное название компании
        /// </summary>
        public string? FullName { get; set; }

        /// <summary>
        /// ОГРН компании
        /// </summary>
        public string? Ogrn { get; set; }

        /// <summary>
        /// КПП компании
        /// </summary>
        public string? Kpp { get; set; }

        /// <summary>
        /// Адрес компании
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// ФИО руководителя
        /// </summary>
        public string? DirectorName { get; set; }

        /// <summary>
        /// Дата регистрации компании
        /// </summary>
        public DateTime? RegistrationDate { get; set; }

        /// <summary>
        /// Компания активна (действующая)
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Полный JSON ответа от DaData
        /// </summary>
        public string? JsonData { get; set; }
    }
}
