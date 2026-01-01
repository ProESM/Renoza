using Renoza.Domain.Entities.CompanyVerification.DaDataApi;

namespace Renoza.Domain.Services.Interfaces.RenozaInterfaces
{
    /// <summary>
    /// Сервис для работы с DaData API
    /// </summary>
    public interface IDaDataApiService
    {
        /// <summary>
        /// Получить информацию о компании по ИНН
        /// </summary>
        /// <param name="inn">ИНН компании</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Данные о компании от DaData</returns>
        Task<DaDataCompanyResponse?> GetCompanyByInnAsync(string inn, CancellationToken cancellationToken = default);
    }
}
