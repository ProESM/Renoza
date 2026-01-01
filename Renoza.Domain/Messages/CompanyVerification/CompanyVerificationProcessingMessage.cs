namespace Renoza.Domain.Messages.CompanyVerification
{
    /// <summary>
    /// Сообщение для обработки верификации компании через DaData API
    /// </summary>
    public class CompanyVerificationProcessingMessage
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
    }
}
