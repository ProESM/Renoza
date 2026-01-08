using Renoza.Common.Base.Helpers;
using Renoza.Domain.Entities.Documents;
using Renoza.Domain.Services.Interfaces.BaseInterfaces;
using Renoza.Infrastructure.Contexts;

namespace Renoza.Domain.Services.Interfaces.RenozaInterfaces
{
    /// <summary>
    /// Интерфейс сервиса работы с документами
    /// </summary>
    public interface IDocumentService : IBaseService<RenozaContext>
    {
        /// <summary>
        /// Создать документ из шаблона
        /// </summary>
        /// <param name="input">Входные данные для создания</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат создания документа</returns>
        Task<Result<DocumentGenerationResult>> CreateAsync(
            GenerateDocumentInput input,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить документ по идентификатору
        /// </summary>
        /// <param name="documentId">Идентификатор документа</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Документ</returns>
        Task<Result<Document>> GetDocumentByIdAsync(Guid documentId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить список документов
        /// </summary>
        /// <param name="templateId">Фильтр по шаблону (опционально)</param>
        /// <param name="customerId">Фильтр по заказчику (опционально)</param>
        /// <param name="orderId">Фильтр по заказу (опционально)</param>
        /// <param name="statusId">Фильтр по статусу (опционально)</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список документов</returns>
        Task<Result<List<Document>>> GetDocumentsAsync(
            Guid? templateId = null,
            Guid? customerId = null,
            Guid? orderId = null,
            short? statusId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Скачать документ
        /// </summary>
        /// <param name="documentId">Идентификатор документа</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат скачивания с потоком данных и метаданными</returns>
        Task<Result<DocumentDownloadResult>> DownloadDocumentAsync(Guid documentId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Удалить документ
        /// </summary>
        /// <param name="documentId">Идентификатор документа</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат операции</returns>
        Task<Result<bool>> DeleteDocumentAsync(Guid documentId, CancellationToken cancellationToken = default);
    }
}
