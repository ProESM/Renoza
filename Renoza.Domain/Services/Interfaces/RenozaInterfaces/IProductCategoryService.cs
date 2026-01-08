using Renoza.Common.Base.Helpers;
using Renoza.Domain.Entities.Products;

namespace Renoza.Domain.Services.Interfaces.RenozaInterfaces
{
    /// <summary>
    /// Сервис для управления категориями товаров и услуг
    /// </summary>
    public interface IProductCategoryService
    {
        /// <summary>
        /// Создать новую категорию
        /// </summary>
        /// <param name="name">Название категории</param>
        /// <param name="description">Описание категории</param>
        /// <param name="parentId">ID родительской категории (null для корневой)</param>
        /// <param name="order">Порядок сортировки</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Созданная категория</returns>
        Task<Result<ProductCategory>> CreateAsync(string name, string? description = null, Guid? parentId = null, int order = 0, CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить категорию по ID
        /// </summary>
        /// <param name="categoryId">ID категории</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Категория</returns>
        Task<Result<ProductCategory>> GetByIdAsync(Guid categoryId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить все активные категории
        /// </summary>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список активных категорий</returns>
        Task<Result<List<ProductCategory>>> GetAllActiveAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить корневые категории (без родителей)
        /// </summary>
        /// <param name="includeInactive">Включить неактивные категории</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список корневых категорий</returns>
        Task<Result<List<ProductCategory>>> GetRootCategoriesAsync(bool includeInactive = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить дочерние категории для указанной родительской
        /// </summary>
        /// <param name="parentId">ID родительской категории</param>
        /// <param name="includeInactive">Включить неактивные категории</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список дочерних категорий</returns>
        Task<Result<List<ProductCategory>>> GetChildCategoriesAsync(Guid parentId, bool includeInactive = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Обновить категорию
        /// </summary>
        /// <param name="categoryId">ID категории</param>
        /// <param name="name">Новое название</param>
        /// <param name="description">Новое описание</param>
        /// <param name="order">Новый порядок сортировки</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Обновленная категория</returns>
        Task<Result<ProductCategory>> UpdateAsync(Guid categoryId, string? name = null, string? description = null, int? order = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Переместить категорию в другую родительскую категорию
        /// </summary>
        /// <param name="categoryId">ID категории для перемещения</param>
        /// <param name="newParentId">ID новой родительской категории (null для перемещения в корень)</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Обновленная категория</returns>
        Task<Result<ProductCategory>> MoveToParentAsync(Guid categoryId, Guid? newParentId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Деактивировать категорию
        /// </summary>
        /// <param name="categoryId">ID категории</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат операции</returns>
        Task<Result<bool>> DeactivateAsync(Guid categoryId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Активировать категорию
        /// </summary>
        /// <param name="categoryId">ID категории</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат операции</returns>
        Task<Result<bool>> ActivateAsync(Guid categoryId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Удалить категорию (только если в ней нет товаров и дочерних категорий)
        /// </summary>
        /// <param name="categoryId">ID категории</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат удаления</returns>
        Task<Result<bool>> DeleteAsync(Guid categoryId, CancellationToken cancellationToken = default);
    }
}
