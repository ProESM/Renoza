using Renoza.Common.Helpers;
using Renoza.Domain.Entities.Verifications;

namespace Renoza.Domain.Services.Interfaces.RenozaInterfaces
{
    /// <summary>
    /// Сервис верификации телефонного номера
    /// </summary>
    public interface IPhoneVerificationService
    {
        /// <summary>
        /// Создать код верификации для телефона
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="phoneNumber">Номер телефона</param>
        /// <param name="phoneCountryCode">Код страны</param>
        /// <returns>Результат создания кода верификации</returns>
        Task<Result<PhoneVerification>> CreateVerificationCodeAsync(Guid userId, string phoneNumber, string phoneCountryCode);

        /// <summary>
        /// Верифицировать телефон по коду
        /// </summary>
        /// <param name="phoneNumber">Номер телефона</param>
        /// <param name="phoneCountryCode">Код страны</param>
        /// <param name="code">Код верификации</param>
        /// <returns>Результат верификации</returns>
        Task<Result<bool>> VerifyPhoneAsync(string phoneNumber, string phoneCountryCode, string code);

        /// <summary>
        /// Получить активную верификацию для пользователя
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns>Активная верификация</returns>
        Task<PhoneVerification?> GetActiveVerificationAsync(Guid userId);

        /// <summary>
        /// Отправить код верификации повторно
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="phoneNumber">Номер телефона</param>
        /// <param name="phoneCountryCode">Код страны</param>
        /// <returns>Результат отправки кода</returns>
        Task<Result<PhoneVerification>> ResendVerificationCodeAsync(Guid userId, string phoneNumber, string phoneCountryCode);
    }
}
