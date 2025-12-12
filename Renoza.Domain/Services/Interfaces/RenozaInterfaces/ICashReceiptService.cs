using Renoza.Common.Helpers;
using Renoza.Domain.Entities.CashReceipts;

namespace Renoza.Domain.Services.Interfaces.RenozaInterfaces
{
    /// <summary>
    /// Сервис для работы с кассовыми чеками
    /// </summary>
    public interface ICashReceiptService
    {
        /// <summary>
        /// Сохранить кассовый чек
        /// </summary>
        /// <param name="input">Входные данные для сохранения чека</param>
        /// <param name="cancellationToken">Токен отмены</param>
        Task<Result<bool>> SaveCashReceiptAsync(SaveCashReceiptInput input, CancellationToken cancellationToken = default);
    }
}
