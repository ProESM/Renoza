using Renoza.Common.Base.Helpers;
using Renoza.Domain.Entities.Products;

namespace Renoza.Domain.Services.Interfaces.RenozaInterfaces
{
    /// <summary>
    /// Сервис для управления товарами и услугами
    /// </summary>
    public interface IProductService
    {
        /// <summary>
        /// Создать новый товар/услугу
        /// </summary>
        /// <param name="categoryId">ID категории</param>
        /// <param name="name">Название товара/услуги</param>
        /// <param name="description">Описание</param>
        /// <param name="measurementUnitId">ID единицы измерения</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Созданный товар/услуга</returns>
        Task<Result<Product>> CreateAsync(Guid categoryId, string name, Guid measurementUnitId, string? description = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить товар/услугу по ID
        /// </summary>
        /// <param name="productId">ID товара/услуги</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Товар/услуга</returns>
        Task<Result<Product>> GetByIdAsync(Guid productId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить все активные товары/услуги
        /// </summary>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список активных товаров/услуг</returns>
        Task<Result<List<Product>>> GetAllActiveAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить товары/услуги по категории
        /// </summary>
        /// <param name="categoryId">ID категории</param>
        /// <param name="includeInactive">Включить неактивные товары</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список товаров/услуг</returns>
        Task<Result<List<Product>>> GetByCategoryAsync(Guid categoryId, bool includeInactive = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Поиск товаров/услуг по названию
        /// </summary>
        /// <param name="searchTerm">Поисковый запрос</param>
        /// <param name="includeInactive">Включить неактивные товары</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список найденных товаров/услуг</returns>
        Task<Result<List<Product>>> SearchByNameAsync(string searchTerm, bool includeInactive = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Обновить товар/услугу
        /// </summary>
        /// <param name="productId">ID товара/услуги</param>
        /// <param name="name">Новое название</param>
        /// <param name="description">Новое описание</param>
        /// <param name="categoryId">Новая категория</param>
        /// <param name="measurementUnitId">Новая единица измерения</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Обновленный товар/услуга</returns>
        Task<Result<Product>> UpdateAsync(Guid productId, string? name = null, string? description = null, Guid? categoryId = null, Guid? measurementUnitId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Деактивировать товар/услугу
        /// </summary>
        /// <param name="productId">ID товара/услуги</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат операции</returns>
        Task<Result<bool>> DeactivateAsync(Guid productId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Активировать товар/услугу
        /// </summary>
        /// <param name="productId">ID товара/услуги</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат операции</returns>
        Task<Result<bool>> ActivateAsync(Guid productId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Удалить товар/услугу (только если нет связанных цен и элементов корзины)
        /// </summary>
        /// <param name="productId">ID товара/услуги</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат удаления</returns>
        Task<Result<bool>> DeleteAsync(Guid productId, CancellationToken cancellationToken = default);
    }
}
