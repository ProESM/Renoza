using Renoza.Domain.Enums;

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
        /// <param name="ipAddress">IP адрес клиента</param>
        /// <param name="inputType">Тип входных данных</param>
        /// <param name="data">Данные чека</param>
        /// <param name="contentType">MIME тип файла (опционально)</param>
        /// <param name="fileName">Имя файла (опционально)</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Идентификатор задания на обработку</returns>
        Task<Guid> UploadCashReceiptAsync(
            string ipAddress,
            ReceiptInputType inputType,
            string data,
            string? contentType = null,
            string? fileName = null,
            CancellationToken cancellationToken = default);
    }
}
