using System.ComponentModel.DataAnnotations;

namespace Renoza.Backend.Models.Auth
{
    /// <summary>
    /// Модель запроса для авторизации
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// Имя пользователя, email или номер телефона
        /// </summary>
        [Required(ErrorMessage = "Имя пользователя обязательно")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Пароль
        /// </summary>
        [Required(ErrorMessage = "Пароль обязателен")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Тип логина: "username", "email", "phone"
        /// </summary>
        public string? LoginType { get; set; }

        /// <summary>
        /// Идентификатор выбранной роли (опционально, если не указана - берется первая активная роль)
        /// </summary>
        public Guid? SelectedRoleId { get; set; }
    }
}
