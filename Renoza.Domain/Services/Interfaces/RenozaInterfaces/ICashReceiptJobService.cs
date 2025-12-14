using Renoza.Common.Helpers;
using Renoza.Domain.Entities.CashReceipts;
using Renoza.Domain.Enums;

namespace Renoza.Domain.Services.Interfaces.RenozaInterfaces
{
    /// <summary>
    /// Сервис для управления запросами на загрузку чеков (Job)
    /// </summary>
    public interface ICashReceiptJobService
    {
        /// <summary>
        /// Создать новый запрос на загрузку чека
        /// </summary>
        /// <param name="input">Входные данные для создания задания</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Идентификатор созданного запроса</returns>
        Task<Result<Guid>> CreateJobAsync(
            CreateCashReceiptJobInput input,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Обновить статус запроса
        /// </summary>
        /// <param name="jobId">Идентификатор запроса</param>
        /// <param name="newStatus">Новый статус</param>
        /// <param name="comment">Комментарий к изменению статуса</param>
        /// <param name="cancellationToken">Токен отмены</param>
        Task<Result<bool>> UpdateJobStatusAsync(
            Guid jobId,
            CashReceiptJobStatus newStatus,
            string? comment = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить запрос по идентификатору
        /// </summary>
        /// <param name="jobId">Идентификатор запроса</param>
        /// <param name="cancellationToken">Токен отмены</param>
        Task<Result<CashReceiptJob>> GetJobByIdAsync(
            Guid jobId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить историю изменений статусов запроса
        /// </summary>
        /// <param name="jobId">Идентификатор запроса</param>
        /// <param name="cancellationToken">Токен отмены</param>
        Task<Result<List<CashReceiptJobHistory>>> GetJobHistoryAsync(
            Guid jobId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Отметить запрос как завершенный
        /// </summary>
        /// <param name="jobId">Идентификатор запроса</param>
        /// <param name="cancellationToken">Токен отмены</param>
        Task<Result<bool>> CompleteJobAsync(
            Guid jobId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Отменить запрос
        /// </summary>
        /// <param name="jobId">Идентификатор запроса</param>
        /// <param name="comment">Причина отмены</param>
        /// <param name="cancellationToken">Токен отмены</param>
        Task<Result<bool>> CancelJobAsync(
            Guid jobId,
            string? comment = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Найти успешно завершенное задание по QR коду
        /// </summary>
        /// <param name="qrSource">QR код чека</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Успешно завершенное задание с JSON данными чека или null</returns>
        Task<Result<CompletedCashReceiptJobResult?>> FindCompletedJobByQrAsync(
            string qrSource,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Удалить временную папку чека в S3 при ошибке обработки
        /// </summary>
        /// <param name="jobId">Идентификатор запроса</param>
        /// <param name="cancellationToken">Токен отмены</param>
        Task CleanupTempStorageAsync(
            Guid jobId,
            CancellationToken cancellationToken = default);
    }
}
