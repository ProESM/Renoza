using Renoza.Domain.Entities.CashReceipts;

namespace Renoza.Domain.Services.Interfaces.RenozaInterfaces
{
    /// <summary>
    /// Сервис для работы с чеками
    /// </summary>
    public interface IReceiptService
    {
        /// <summary>
        /// Загрузить кассовый чек (отправить в очередь обработки)
        /// </summary>
        /// <param name="input">Входные данные для загрузки чека</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Идентификатор задания на обработку</returns>
        Task<Guid> UploadCashReceiptAsync(
            UploadCashReceiptInput input,
            CancellationToken cancellationToken = default);
    }
}
