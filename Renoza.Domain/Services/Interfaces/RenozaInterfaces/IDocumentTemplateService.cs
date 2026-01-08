using Renoza.Common.Base.Helpers;
using Renoza.Domain.Entities.Documents;
using Renoza.Domain.Services.Interfaces.BaseInterfaces;
using Renoza.Infrastructure.Contexts;

namespace Renoza.Domain.Services.Interfaces.RenozaInterfaces
{
    /// <summary>
    /// Интерфейс сервиса работы с шаблонами документов
    /// </summary>
    public interface IDocumentTemplateService : IBaseService<RenozaContext>
    {
        /// <summary>
        /// Загрузить шаблон документа
        /// </summary>
        /// <param name="input">Входные данные для загрузки шаблона</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Идентификатор созданного шаблона</returns>
        Task<Result<Guid>> UploadTemplateAsync(UploadTemplateInput input, CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить шаблон по идентификатору
        /// </summary>
        /// <param name="templateId">Идентификатор шаблона</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Шаблон документа</returns>
        Task<Result<DocumentTemplate>> GetTemplateByIdAsync(Guid templateId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить список шаблонов
        /// </summary>
        /// <param name="templateTypeId">Фильтр по типу шаблона (опционально)</param>
        /// <param name="isActive">Фильтр по активности (опционально)</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список шаблонов</returns>
        Task<Result<List<DocumentTemplate>>> GetTemplatesAsync(
            short? templateTypeId = null,
            bool? isActive = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Удалить шаблон
        /// </summary>
        /// <param name="templateId">Идентификатор шаблона</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат операции</returns>
        Task<Result<bool>> DeleteTemplateAsync(Guid templateId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Скачать файл шаблона
        /// </summary>
        /// <param name="templateId">Идентификатор шаблона</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Поток с данными файла</returns>
        Task<Result<Stream>> DownloadTemplateFileAsync(Guid templateId, CancellationToken cancellationToken = default);
    }
}
