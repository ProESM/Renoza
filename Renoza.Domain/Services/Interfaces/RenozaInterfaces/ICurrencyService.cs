using Renoza.Common.Base.Helpers;
using Renoza.Domain.Entities.Products;

namespace Renoza.Domain.Services.Interfaces.RenozaInterfaces
{
    /// <summary>
    /// Сервис для работы со справочником валют
    /// </summary>
    public interface ICurrencyService
    {
        /// <summary>
        /// Получить все валюты
        /// </summary>
        /// <param name="includeInactive">Включить неактивные валюты</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список валют</returns>
        Task<Result<List<Currency>>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить валюту по ID
        /// </summary>
        /// <param name="id">ID валюты</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Валюта</returns>
        Task<Result<Currency>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить валюту по коду
        /// </summary>
        /// <param name="code">Код валюты (например, RUB, USD, EUR)</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Валюта</returns>
        Task<Result<Currency>> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    }
}
