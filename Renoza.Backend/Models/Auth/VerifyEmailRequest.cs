using System.ComponentModel.DataAnnotations;

namespace Renoza.Backend.Models.Auth
{
    /// <summary>
    /// Модель запроса для верификации email
    /// </summary>
    public class VerifyEmailRequest
    {
        /// <summary>
        /// Email для верификации
        /// </summary>
        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный формат email")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Код верификации
        /// </summary>
        [Required(ErrorMessage = "Код верификации обязателен")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Код верификации должен состоять из 6 символов")]
        public string Code { get; set; } = string.Empty;
    }
}
