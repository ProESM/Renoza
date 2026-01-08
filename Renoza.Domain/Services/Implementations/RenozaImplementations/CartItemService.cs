using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Renoza.Common.Base.Helpers;
using Renoza.Domain.Entities.Cart;
using Renoza.Domain.Services.Implementations.BaseImplementations;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;
using Renoza.Infrastructure.Contexts;
using Renoza.Infrastructure.Entities.Renoza;
using Renoza.Infrastructure.Repositories.Interfaces.BaseInterfaces;

namespace Renoza.Domain.Services.Implementations.RenozaImplementations
{
    /// <summary>
    /// Сервис для управления элементами корзины покупок
    /// </summary>
    public class CartItemService : BaseService<RenozaContext>, ICartItemService
    {
        /// <summary>
        /// Репозиторий для работы с элементами корзины покупок
        /// </summary>
        private readonly IEntityWithIdRepository<CartItemDao, Guid> _cartItemRepository;
        
        /// <summary>
        /// Маппер для преобразования между DAO и Domain сущностями
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Сервис для управления элементами корзины покупок
        /// </summary>
        /// <param name="dbContext">Контекст базы данных</param>
        /// <param name="cartItemRepository">Репозиторий для работы с элементами корзины покупок</param>
        /// <param name="mapper">Маппер для преобразования между DAO и Domain сущностями</param>
        public CartItemService(
            RenozaContext dbContext,
            IEntityWithIdRepository<CartItemDao, Guid> cartItemRepository,
            IMapper mapper) : base(dbContext)
        {
            _cartItemRepository = cartItemRepository;
            _mapper = mapper;
        }

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
        public async Task<Result<CartItem>> AddToCartAsync(
            Guid userId, Guid profileId, Guid productId, decimal quantity,
            string? notes = null, CancellationToken cancellationToken = default)
        {
            var existingCartItemDao = await _cartItemRepository.GetQueryable()
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.ProfileId == profileId &&
                    x.ProductId == productId,
                    cancellationToken);

            if (existingCartItemDao != null)
            {
                existingCartItemDao.Quantity += quantity;
                existingCartItemDao.Notes = notes ?? existingCartItemDao.Notes;
                existingCartItemDao.UpdatedAt = DateTime.UtcNow;

                _cartItemRepository.Update(existingCartItemDao);
                await SaveChangesAsync(cancellationToken);

                var updatedItem = _mapper.Map<CartItem>(existingCartItemDao);
                return Result<CartItem>.Success(updatedItem);
            }

            var cartItemDao = new CartItemDao
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ProfileId = profileId,
                ProductId = productId,
                Quantity = quantity,
                Notes = notes,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _cartItemRepository.CreateAsync(cartItemDao, cancellationToken);
            await SaveChangesAsync(cancellationToken);

            var cartItem = _mapper.Map<CartItem>(cartItemDao);
            return Result<CartItem>.Success(cartItem);
        }

        /// <summary>
        /// Обновить количество товара в корзине
        /// </summary>
        /// <param name="cartItemId">ID элемента корзины</param>
        /// <param name="quantity">Новое количество</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Обновленный элемент корзины</returns>
        public async Task<Result<CartItem>> UpdateQuantityAsync(
            Guid cartItemId, decimal quantity, CancellationToken cancellationToken = default)
        {
            var cartItemDao = await _cartItemRepository.GetByIdAsync(cartItemId, cancellationToken);

            if (cartItemDao == null)
                return Result<CartItem>.Failure("Элемент корзины не найден");

            if (quantity <= 0)
                return Result<CartItem>.Failure("Количество должно быть больше нуля");

            cartItemDao.Quantity = quantity;
            cartItemDao.UpdatedAt = DateTime.UtcNow;

            _cartItemRepository.Update(cartItemDao);
            await SaveChangesAsync(cancellationToken);

            var cartItem = _mapper.Map<CartItem>(cartItemDao);
            return Result<CartItem>.Success(cartItem);
        }

        /// <summary>
        /// Удалить товар из корзины
        /// </summary>
        /// <param name="cartItemId">ID элемента корзины</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат удаления</returns>
        public async Task<Result<bool>> RemoveFromCartAsync(
            Guid cartItemId, CancellationToken cancellationToken = default)
        {
            var cartItemDao = await _cartItemRepository.GetByIdAsync(cartItemId, cancellationToken);

            if (cartItemDao == null)
                return Result<bool>.Failure("Элемент корзины не найден");

            _cartItemRepository.Delete(cartItemDao);
            await SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }

        /// <summary>
        /// Получить все элементы корзины пользователя
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список элементов корзины</returns>
        public async Task<Result<List<CartItem>>> GetCartItemsAsync(
            Guid userId, CancellationToken cancellationToken = default)
        {
            var cartItemsDaos = await _cartItemRepository.GetQueryable()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);

            var cartItems = _mapper.Map<List<CartItem>>(cartItemsDaos);
            return Result<List<CartItem>>.Success(cartItems);
        }

        /// <summary>
        /// Очистить корзину пользователя
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат очистки</returns>
        public async Task<Result<bool>> ClearCartAsync(
            Guid userId, CancellationToken cancellationToken = default)
        {
            var cartItemsDaos = await _cartItemRepository.GetQueryable()
                .Where(x => x.UserId == userId)
                .ToListAsync(cancellationToken);

            _cartItemRepository.DeleteRange(cartItemsDaos);
            await SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
