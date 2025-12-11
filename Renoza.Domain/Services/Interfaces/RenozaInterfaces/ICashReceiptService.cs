using Renoza.Common.Helpers;
using Renoza.Domain.Enums;

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
        /// <param name="jobId">Идентификатор задания</param>
        /// <param name="inputType">Тип входных данных</param>
        /// <param name="data">Данные чека</param>
        /// <param name="contentType">MIME тип файла (опционально)</param>
        /// <param name="fileName">Имя файла (опционально)</param>
        /// <param name="existingJobId">Идентификатор ранее обработанного задания (опционально)</param>
        /// <param name="cancellationToken">Токен отмены</param>
        Task<Result<bool>> SaveCashReceiptAsync(Guid jobId,
            ReceiptInputType inputType,
            string data,
            string? contentType = null,
            string? fileName = null,
            Guid? existingJobId = null,
            CancellationToken cancellationToken = default);
    }
}
