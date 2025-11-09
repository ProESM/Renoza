using System.ComponentModel.DataAnnotations;

namespace Renoza.Backend.Models.Password
{
    /// <summary>
    /// Модель запроса изменения пароля
    /// </summary>
    public class PasswordChangeRequest
    {
        /// <summary>
        /// Текущий пароль
        /// </summary>
        [Required(ErrorMessage = "Текущий пароль обязателен")]
        public string CurrentPassword { get; set; } = string.Empty;
        /// <summary>
        /// Новый пароль
        /// </summary>
        [Required(ErrorMessage = "Новый пароль обязателен")]
        public string NewPassword { get; set; } = string.Empty;
        /// <summary>
        /// Подтвержденный новый пароль
        /// </summary>
        [Required(ErrorMessage = "Подтвержденный новый пароль обязателен")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
