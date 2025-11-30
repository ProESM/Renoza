namespace Renoza.Domain.Services.Interfaces.RenozaInterfaces
{
    /// <summary>
    /// Сервис для отправки email
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Отправить email
        /// </summary>
        /// <param name="to">Email получателя</param>
        /// <param name="subject">Тема письма</param>
        /// <param name="htmlBody">HTML тело письма</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>True если письмо успешно отправлено</returns>
        Task<bool> SendEmailAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default);

        /// <summary>
        /// Отправить email с кодом верификации
        /// </summary>
        /// <param name="to">Email получателя</param>
        /// <param name="code">Код верификации</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>True если письмо успешно отправлено</returns>
        Task<bool> SendVerificationCodeAsync(string to, string code, CancellationToken cancellationToken = default);
    }
}
