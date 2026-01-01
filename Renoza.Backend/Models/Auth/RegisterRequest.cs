using System.ComponentModel.DataAnnotations;
using Renoza.Backend.Validators;

namespace Renoza.Backend.Models.Auth
{
    /// <summary>
    /// Модель запроса для регистрации нового пользователя
    /// </summary>
    public class RegisterRequest
    {
        /// <summary>
        /// Имя пользователя (логин)
        /// </summary>
        [Required(ErrorMessage = "Имя пользователя обязательно")]
        [MinLength(3, ErrorMessage = "Имя пользователя должно быть не менее 3 символов")]
        [MaxLength(50, ErrorMessage = "Имя пользователя не может быть длиннее 50 символов")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Отображаемое имя
        /// </summary>
        [Required(ErrorMessage = "Отображаемое имя обязательно")]
        [MaxLength(100, ErrorMessage = "Отображаемое имя не может быть длиннее 100 символов")]
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// Email
        /// </summary>
        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный формат email")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Номер телефона
        /// </summary>
        [Required(ErrorMessage = "Номер телефона обязателен")]
        [Phone(ErrorMessage = "Некорректный формат номера телефона")]
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// Код страны для телефона
        /// </summary>
        [Required(ErrorMessage = "Код страны обязателен")]
        public string PhoneCountryCode { get; set; } = string.Empty;

        /// <summary>
        /// Пароль
        /// </summary>
        [Required(ErrorMessage = "Пароль обязателен")]
        [MinLength(6, ErrorMessage = "Пароль должен быть не менее 6 символов")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Роль пользователя (Customer, Worker, TechnicalSupervisor)
        /// </summary>
        [Required(ErrorMessage = "Роль пользователя обязательна")]
        [RegularExpression(@"^(Customer|Worker|TechnicalSupervisor)$", ErrorMessage = "Роль должна быть одной из: Customer, Worker, TechnicalSupervisor")]
        public string UserRole { get; set; } = string.Empty;

        /// <summary>
        /// ИНН компании (обязательно для Worker и TechnicalSupervisor)
        /// </summary>
        [RequiredIfRole("Worker", "TechnicalSupervisor", ErrorMessage = "ИНН компании обязателен для ролей Worker и TechnicalSupervisor")]
        [StringLength(12, MinimumLength = 10, ErrorMessage = "ИНН должен быть 10 или 12 цифр")]
        [RegularExpression(@"^\d{10}$|^\d{12}$", ErrorMessage = "ИНН должен содержать только цифры (10 для ИП, 12 для юрлица)")]
        public string? CompanyInn { get; set; }
    }
}
