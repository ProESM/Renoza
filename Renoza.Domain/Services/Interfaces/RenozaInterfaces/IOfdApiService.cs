using Renoza.Domain.Entities.CashReceipts.OfdApi;

namespace Renoza.Domain.Services.Interfaces.RenozaInterfaces
{
    /// <summary>
    /// Сервис для работы с API OFD.ru
    /// </summary>
    public interface IOfdApiService
    {
        /// <summary>
        /// Получить данные чека по QR коду
        /// </summary>
        /// <param name="request">Параметры запроса</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Данные чека от OFD.ru</returns>
        Task<OfdApiResponse> GetReceiptAsync(OfdApiRequest request, CancellationToken cancellationToken = default);
    }
}
