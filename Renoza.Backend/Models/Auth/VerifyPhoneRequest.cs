using System.ComponentModel.DataAnnotations;

namespace Renoza.Backend.Models.Auth
{
    /// <summary>
    /// Модель запроса для верификации телефона
    /// </summary>
    public class VerifyPhoneRequest
    {
        /// <summary>
        /// Номер телефона для верификации
        /// </summary>
        [Required(ErrorMessage = "Номер телефона обязателен")]
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// Код страны
        /// </summary>
        [Required(ErrorMessage = "Код страны обязателен")]
        public string PhoneCountryCode { get; set; } = string.Empty;

        /// <summary>
        /// Код верификации
        /// </summary>
        [Required(ErrorMessage = "Код верификации обязателен")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Код верификации должен состоять из 6 символов")]
        public string Code { get; set; } = string.Empty;
    }
}
