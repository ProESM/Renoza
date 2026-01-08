using Renoza.Common.Base.Helpers;
using Renoza.Domain.Entities.Cart;

namespace Renoza.Domain.Services.Interfaces.RenozaInterfaces
{
    /// <summary>
    /// Сервис для управления элементами корзины покупок
    /// </summary>
    public interface ICartItemService
    {
        /// <summary>
        /// Добавить товар в корзину
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <param name="profileId">ID профиля исполнителя</param>
        /// <param name="productId">ID товара/услуги</param>
        /// <param name="quantity">Количество</param>
        /// <param name="notes">Примечания к заказу</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Элемент корзины</returns>
        Task<Result<CartItem>> AddToCartAsync(Guid userId, Guid profileId, Guid productId, decimal quantity, string? notes = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Обновить количество товара в корзине
        /// </summary>
        /// <param name="cartItemId">ID элемента корзины</param>
        /// <param name="quantity">Новое количество</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Обновленный элемент корзины</returns>
        Task<Result<CartItem>> UpdateQuantityAsync(Guid cartItemId, decimal quantity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Удалить товар из корзины
        /// </summary>
        /// <param name="cartItemId">ID элемента корзины</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат удаления</returns>
        Task<Result<bool>> RemoveFromCartAsync(Guid cartItemId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить все элементы корзины пользователя
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список элементов корзины</returns>
        Task<Result<List<CartItem>>> GetCartItemsAsync(Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Очистить корзину пользователя
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат очистки</returns>
        Task<Result<bool>> ClearCartAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
