using Renoza.Common.Base.Helpers;
using Renoza.Domain.Entities.Products;

namespace Renoza.Domain.Services.Interfaces.RenozaInterfaces
{
    /// <summary>
    /// Сервис для работы со справочником единиц измерения
    /// </summary>
    public interface IMeasurementUnitService
    {
        /// <summary>
        /// Получить все единицы измерения
        /// </summary>
        /// <param name="includeInactive">Включить неактивные единицы</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список единиц измерения</returns>
        Task<Result<List<MeasurementUnit>>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить единицу измерения по ID
        /// </summary>
        /// <param name="id">ID единицы измерения</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Единица измерения</returns>
        Task<Result<MeasurementUnit>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить единицу измерения по коду
        /// </summary>
        /// <param name="code">Код единицы измерения (например, шт, м², кг)</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Единица измерения</returns>
        Task<Result<MeasurementUnit>> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить единицы измерения по типу
        /// </summary>
        /// <param name="typeId">ID типа единицы измерения</param>
        /// <param name="includeInactive">Включить неактивные единицы</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список единиц измерения</returns>
        Task<Result<List<MeasurementUnit>>> GetByTypeAsync(Guid typeId, bool includeInactive = false, CancellationToken cancellationToken = default);
    }
}
