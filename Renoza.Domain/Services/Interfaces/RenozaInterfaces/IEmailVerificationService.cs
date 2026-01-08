using Renoza.Common.Base.Helpers;
using Renoza.Domain.Entities.Verifications;

namespace Renoza.Domain.Services.Interfaces.RenozaInterfaces
{
    /// <summary>
    /// Сервис верификации электронной почты
    /// </summary>
    public interface IEmailVerificationService
    {
        /// <summary>
        /// Создать код верификации для email
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="email">Адрес электронной почты</param>
        /// <returns>Результат создания кода верификации</returns>
        Task<Result<EmailVerification>> CreateVerificationCodeAsync(Guid userId, string email);

        /// <summary>
        /// Верифицировать email по коду
        /// </summary>
        /// <param name="email">Адрес электронной почты</param>
        /// <param name="code">Код верификации</param>
        /// <returns>Результат верификации</returns>
        Task<Result<bool>> VerifyEmailAsync(string email, string code);

        /// <summary>
        /// Получить активную верификацию для пользователя
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns>Активная верификация</returns>
        Task<EmailVerification?> GetActiveVerificationAsync(Guid userId);

        /// <summary>
        /// Отправить код верификации повторно
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="email">Адрес электронной почты</param>
        /// <returns>Результат отправки кода</returns>
        Task<Result<EmailVerification>> ResendVerificationCodeAsync(Guid userId, string email);
    }
}
