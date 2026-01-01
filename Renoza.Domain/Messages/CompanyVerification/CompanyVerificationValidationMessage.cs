namespace Renoza.Domain.Messages.CompanyVerification
{
    /// <summary>
    /// Сообщение для валидации задания на верификацию компании
    /// </summary>
    public class CompanyVerificationValidationMessage
    {
        /// <summary>
        /// ID задания на верификацию
        /// </summary>
        public Guid JobId { get; set; }

        /// <summary>
        /// ID профиля компании
        /// </summary>
        public Guid CompanyProfileId { get; set; }

        /// <summary>
        /// ИНН компании для верификации
        /// </summary>
        public string Inn { get; set; } = string.Empty;

        /// <summary>
        /// IP адрес, с которого создано задание
        /// </summary>
        public string IpAddress { get; set; } = string.Empty;
    }
}
