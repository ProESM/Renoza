using System.ComponentModel.DataAnnotations;

namespace Renoza.Domain.Options
{
    /// <summary>
    /// Настройки SMTP для отправки email
    /// </summary>
    public class EmailOptions
    {
        /// <summary>
        /// SMTP сервер
        /// </summary>
        [Required]
        public string SmtpServer { get; set; } = string.Empty;

        /// <summary>
        /// SMTP порт
        /// </summary>
        [Range(1, 65535)]
        public int SmtpPort { get; set; } = 587;

        /// <summary>
        /// Использовать SSL/TLS
        /// </summary>
        public bool UseSsl { get; set; } = true;

        /// <summary>
        /// Имя пользователя для SMTP
        /// </summary>
        [Required]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Пароль для SMTP
        /// </summary>
        [Required]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Email отправителя
        /// </summary>
        [Required]
        [EmailAddress]
        public string FromEmail { get; set; } = string.Empty;

        /// <summary>
        /// Имя отправителя
        /// </summary>
        [Required]
        public string FromName { get; set; } = string.Empty;

        /// <summary>
        /// Таймаут подключения в миллисекундах
        /// </summary>
        [Range(1000, 60000)]
        public int TimeoutMs { get; set; } = 30000;
    }
}
