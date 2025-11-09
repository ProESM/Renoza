namespace Renoza.Backend.Models.Auth
{
    /// <summary>
    /// Модель ответа при успешной авторизации
    /// </summary>
    public class AuthResponse
    {
        /// <summary>
        /// JWT токен
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// Дата истечения токена
        /// </summary>
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// Тип токена (обычно "Bearer")
        /// </summary>
        public string TokenType { get; set; } = "Bearer";

        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string Username { get; set; } = string.Empty;
    }
}
