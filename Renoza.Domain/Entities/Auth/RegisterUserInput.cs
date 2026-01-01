namespace Renoza.Domain.Entities.Auth
{
    /// <summary>
    /// Входные данные для регистрации пользователя (Domain слой)
    /// </summary>
    public class RegisterUserInput
    {
        /// <summary>
        /// Имя пользователя (логин)
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Отображаемое имя
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Номер телефона
        /// </summary>
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// Код страны для телефона
        /// </summary>
        public string PhoneCountryCode { get; set; } = string.Empty;

        /// <summary>
        /// Пароль
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Роль пользователя (Customer, Worker, TechnicalSupervisor)
        /// </summary>
        public string UserRole { get; set; } = string.Empty;

        /// <summary>
        /// ИНН компании (обязательно для Worker и TechnicalSupervisor)
        /// </summary>
        public string? CompanyInn { get; set; }

        /// <summary>
        /// IP адрес для создания задания на верификацию
        /// </summary>
        public string? IpAddress { get; set; }
    }
}
