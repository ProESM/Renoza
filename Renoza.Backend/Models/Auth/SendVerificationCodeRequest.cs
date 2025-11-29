using System.ComponentModel.DataAnnotations;

namespace Renoza.Backend.Models.Auth
{
    /// <summary>
    /// Модель запроса для отправки кода верификации
    /// </summary>
    public class SendVerificationCodeRequest
    {
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        [Required(ErrorMessage = "Идентификатор пользователя обязателен")]
        public Guid UserId { get; set; }

        /// <summary>
        /// Тип верификации: "email" или "phone"
        /// </summary>
        [Required(ErrorMessage = "Тип верификации обязателен")]
        public string VerificationType { get; set; } = string.Empty;

        /// <summary>
        /// Email (если тип верификации - email)
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Номер телефона (если тип верификации - phone)
        /// </summary>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Код страны (если тип верификации - phone)
        /// </summary>
        public string? PhoneCountryCode { get; set; }
    }
}
