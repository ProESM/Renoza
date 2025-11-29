namespace Renoza.Backend.Models.Auth
{
    /// <summary>
    /// Модель ответа при успешной регистрации
    /// </summary>
    public class RegisterResponse
    {
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Email пользователя
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Номер телефона
        /// </summary>
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// Код страны
        /// </summary>
        public string PhoneCountryCode { get; set; } = string.Empty;

        /// <summary>
        /// Сообщение о необходимости верификации
        /// </summary>
        public string Message { get; set; } = "Регистрация успешна. Пожалуйста, подтвердите email и номер телефона.";
    }
}
