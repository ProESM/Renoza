using Renoza.Common.Base.Helpers;
using Renoza.Domain.Entities.Products;

namespace Renoza.Domain.Services.Interfaces.RenozaInterfaces
{
    /// <summary>
    /// Сервис для управления ценами профилей на товары/услуги с проверкой прав
    /// </summary>
    public interface IProfileProductPriceService
    {
        /// <summary>
        /// Установить цену для профиля на товар/услугу
        /// </summary>
        /// <param name="currentUserId">ID текущего пользователя (для проверки прав)</param>
        /// <param name="profileId">ID профиля (технадзор или рабочий)</param>
        /// <param name="productId">ID товара/услуги</param>
        /// <param name="currencyId">ID валюты</param>
        /// <param name="price">Цена</param>
        /// <param name="startDate">Дата начала действия цены</param>
        /// <param name="endDate">Дата окончания действия цены (null для бессрочной)</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Установленная цена</returns>
        Task<Result<ProfileProductPrice>> SetPriceAsync(Guid currentUserId, Guid profileId, Guid productId, Guid currencyId, decimal price, DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить текущую цену для профиля на товар/услугу
        /// </summary>
        /// <param name="profileId">ID профиля</param>
        /// <param name="productId">ID товара/услуги</param>
        /// <param name="date">Дата, на которую нужна цена (null для текущей даты)</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Текущая цена</returns>
        Task<Result<ProfileProductPrice>> GetCurrentPriceAsync(Guid profileId, Guid productId, DateTime? date = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить все цены для профиля
        /// </summary>
        /// <param name="profileId">ID профиля</param>
        /// <param name="includeExpired">Включить истекшие цены</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список цен</returns>
        Task<Result<List<ProfileProductPrice>>> GetPricesByProfileAsync(Guid profileId, bool includeExpired = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить историю цен для профиля на конкретный товар/услугу
        /// </summary>
        /// <param name="profileId">ID профиля</param>
        /// <param name="productId">ID товара/услуги</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>История цен</returns>
        Task<Result<List<ProfileProductPrice>>> GetPriceHistoryAsync(Guid profileId, Guid productId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Обновить цену для профиля на товар/услугу
        /// </summary>
        /// <param name="currentUserId">ID текущего пользователя (для проверки прав)</param>
        /// <param name="profileId">ID профиля</param>
        /// <param name="productId">ID товара/услуги</param>
        /// <param name="oldStartDate">Текущая дата начала действия цены</param>
        /// <param name="newPrice">Новая цена</param>
        /// <param name="newCurrencyId">Новая валюта</param>
        /// <param name="newEndDate">Новая дата окончания</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Обновленная цена</returns>
        Task<Result<ProfileProductPrice>> UpdatePriceAsync(Guid currentUserId, Guid profileId, Guid productId, DateTime oldStartDate, decimal? newPrice = null, Guid? newCurrencyId = null, DateTime? newEndDate = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Завершить действие цены (установить EndDate на текущую дату)
        /// </summary>
        /// <param name="currentUserId">ID текущего пользователя (для проверки прав)</param>
        /// <param name="profileId">ID профиля</param>
        /// <param name="productId">ID товара/услуги</param>
        /// <param name="startDate">Дата начала действия цены</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат операции</returns>
        Task<Result<bool>> TerminatePriceAsync(Guid currentUserId, Guid profileId, Guid productId, DateTime startDate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Удалить цену для профиля на товар/услугу
        /// </summary>
        /// <param name="currentUserId">ID текущего пользователя (для проверки прав)</param>
        /// <param name="profileId">ID профиля</param>
        /// <param name="productId">ID товара/услуги</param>
        /// <param name="startDate">Дата начала действия цены</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат удаления</returns>
        Task<Result<bool>> DeletePriceAsync(Guid currentUserId, Guid profileId, Guid productId, DateTime startDate, CancellationToken cancellationToken = default);
    }
}
