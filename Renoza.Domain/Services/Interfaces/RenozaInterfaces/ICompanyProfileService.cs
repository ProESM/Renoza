using Renoza.Common.Helpers;
using Renoza.Domain.Entities.CompanyProfiles;

namespace Renoza.Domain.Services.Interfaces.RenozaInterfaces
{
    /// <summary>
    /// Сервис для работы с профилями компаний
    /// </summary>
    public interface ICompanyProfileService
    {
        /// <summary>
        /// Создать профиль компании
        /// </summary>
        /// <param name="inn">ИНН компании</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Созданный профиль компании</returns>
        Task<Result<CompanyProfile>> CreateCompanyProfileAsync(string inn,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить профиль компании по ID
        /// </summary>
        /// <param name="id">ID профиля компании</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Профиль компании</returns>
        Task<Result<CompanyProfile>> GetCompanyProfileByIdAsync(Guid id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить профиль компании по ИНН
        /// </summary>
        /// <param name="inn">ИНН компании</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Профиль компании</returns>
        Task<Result<CompanyProfile>> GetCompanyProfileByInnAsync(string inn,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Обновить статус верификации компании и продублировать ключевые поля
        /// </summary>
        /// <param name="companyProfileId">ID профиля компании</param>
        /// <param name="companyVerificationId">ID результата верификации</param>
        /// <param name="isVerified">Статус верификации</param>
        /// <param name="companyType">Тип компании (0 - неизвестно, 1 - юр.лицо, 2 - ИП)</param>
        /// <param name="updateData">Дублированные поля для обновления профиля</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат операции</returns>
        Task<Result<CompanyProfile>> UpdateVerificationStatusAsync(
            Guid companyProfileId,
            Guid companyVerificationId,
            bool isVerified,
            short companyType,
            CompanyProfileUpdateData? updateData = null,
            CancellationToken cancellationToken = default);
    }
}
