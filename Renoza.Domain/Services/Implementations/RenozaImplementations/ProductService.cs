using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Renoza.Common.Base.Helpers;
using Renoza.Domain.Entities.Products;
using Renoza.Domain.Services.Implementations.BaseImplementations;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;
using Renoza.Infrastructure.Contexts;
using Renoza.Infrastructure.Entities.Renoza;
using Renoza.Infrastructure.Repositories.Interfaces.BaseInterfaces;

namespace Renoza.Domain.Services.Implementations.RenozaImplementations
{
    /// <summary>
    /// Сервис для управления товарами и услугами
    /// </summary>
    public class ProductService : BaseService<RenozaContext>, IProductService
    {
        #region Репозитории

        /// <summary>
        /// Репозиторий для работы с товарами
        /// </summary>
        private readonly IEntityWithIdRepository<ProductDao, Guid> _productRepository;

        /// <summary>
        /// Репозиторий для работы с категориями
        /// </summary>
        private readonly IEntityWithIdRepository<ProductCategoryDao, Guid> _categoryRepository;

        /// <summary>
        /// Репозиторий для работы с единицами измерения
        /// </summary>
        private readonly IEntityWithIdRepository<MeasurementUnitDao, Guid> _measurementUnitRepository;

        /// <summary>
        /// Репозиторий для работ с ценами на товары
        /// </summary>
        private readonly IEntityRepository<ProfileProductPriceDao> _priceRepository;

        /// <summary>
        /// Репозиторий для работы с корзиной
        /// </summary>
        private readonly IEntityWithIdRepository<CartItemDao, Guid> _cartItemRepository;

        #endregion

        #region Мапперы

        /// <summary>
        /// Маппер для преобразования между DAO и Domain сущностями
        /// </summary>
        private readonly IMapper _mapper;

        #endregion

        /// <summary>
        /// Сервис для управления товарами и услугами
        /// </summary>
        /// <param name="context">Контекст базы данных</param>
        /// <param name="productRepository">Репозиторий для работы с товарами</param>
        /// <param name="categoryRepository">Репозиторий для работы с категориями</param>
        /// <param name="measurementUnitRepository">Репозиторий для работы с единицами измерения</param>
        /// <param name="priceRepository">Репозиторий для работы с ценами</param>
        /// <param name="cartItemRepository">Репозиторий для работы с корзиной</param>
        /// <param name="mapper">Маппер для преобразования между DAO и Domain сущностями</param>
        public ProductService(
            RenozaContext context,
            IEntityWithIdRepository<ProductDao, Guid> productRepository,
            IEntityWithIdRepository<ProductCategoryDao, Guid> categoryRepository,
            IEntityWithIdRepository<MeasurementUnitDao, Guid> measurementUnitRepository,
            IEntityRepository<ProfileProductPriceDao> priceRepository,
            IEntityWithIdRepository<CartItemDao, Guid> cartItemRepository,
            IMapper mapper) : base(context)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _measurementUnitRepository = measurementUnitRepository;
            _priceRepository = priceRepository;
            _cartItemRepository = cartItemRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Создать новый товар/услугу
        /// </summary>
        /// <param name="categoryId">ID категории</param>
        /// <param name="name">Название товара/услуги</param>
        /// <param name="measurementUnitId">ID единицы измерения</param>
        /// <param name="description">Описание</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Созданный товар/услуга</returns>
        public async Task<Result<Product>> CreateAsync(
            Guid categoryId, string name, Guid measurementUnitId, string? description = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result<Product>.Failure("Название товара/услуги не может быть пустым");

            // Проверка существования категории
            var categoryExists = await _categoryRepository.GetQueryable()
                .AnyAsync(x => x.Id == categoryId && x.IsActive, cancellationToken);

            if (!categoryExists)
                return Result<Product>.Failure("Категория не найдена или неактивна");

            // Проверка существования единицы измерения
            var measurementUnitExists = await _measurementUnitRepository.GetQueryable()
                .AnyAsync(x => x.Id == measurementUnitId, cancellationToken);

            if (!measurementUnitExists)
                return Result<Product>.Failure("Единица измерения не найдена");

            var productDao = new ProductDao
            {
                Id = Guid.NewGuid(),
                CategoryId = categoryId,
                Name = name,
                Description = description,
                MeasurementUnitId = measurementUnitId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _productRepository.CreateAsync(productDao, cancellationToken);
            await SaveChangesAsync(cancellationToken);

            var product = _mapper.Map<Product>(productDao);
            return Result<Product>.Success(product);
        }

        /// <summary>
        /// Получить товар/услугу по ID
        /// </summary>
        /// <param name="productId">ID товара/услуги</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Товар/услуга</returns>
        public async Task<Result<Product>> GetByIdAsync(
            Guid productId, CancellationToken cancellationToken = default)
        {
            var productDao = await _productRepository.GetByIdAsync(productId, cancellationToken);

            if (productDao == null)
                return Result<Product>.Failure("Товар/услуга не найдена");

            var product = _mapper.Map<Product>(productDao);
            return Result<Product>.Success(product);
        }

        /// <summary>
        /// Получить все активные товары/услуги
        /// </summary>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список активных товаров/услуг</returns>
        public async Task<Result<List<Product>>> GetAllActiveAsync(
            CancellationToken cancellationToken = default)
        {
            var productsDaos = await _productRepository.GetQueryable()
                .Where(x => x.IsActive)
                .OrderBy(x => x.CategoryId)
                .ThenBy(x => x.Name)
                .ToListAsync(cancellationToken);

            var products = _mapper.Map<List<Product>>(productsDaos);
            return Result<List<Product>>.Success(products);
        }

        /// <summary>
        /// Получить товары/услуги по категории
        /// </summary>
        /// <param name="categoryId">ID категории</param>
        /// <param name="includeInactive">Включить неактивные товары</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список товаров/услуг</returns>
        public async Task<Result<List<Product>>> GetByCategoryAsync(
            Guid categoryId, bool includeInactive = false, CancellationToken cancellationToken = default)
        {
            var query = _productRepository.GetQueryable()
                .Where(x => x.CategoryId == categoryId);

            if (!includeInactive)
                query = query.Where(x => x.IsActive);

            var productsDaos = await query
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);

            var products = _mapper.Map<List<Product>>(productsDaos);
            return Result<List<Product>>.Success(products);
        }

        /// <summary>
        /// Поиск товаров/услуг по названию
        /// </summary>
        /// <param name="searchTerm">Поисковый запрос</param>
        /// <param name="includeInactive">Включить неактивные товары</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список найденных товаров/услуг</returns>
        public async Task<Result<List<Product>>> SearchByNameAsync(
            string searchTerm, bool includeInactive = false, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return Result<List<Product>>.Success(new List<Product>());

            var query = _productRepository.GetQueryable()
                .Where(x => EF.Functions.ILike(x.Name, $"%{searchTerm}%"));

            if (!includeInactive)
                query = query.Where(x => x.IsActive);

            var productsDaos = await query
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);

            var products = _mapper.Map<List<Product>>(productsDaos);
            return Result<List<Product>>.Success(products);
        }

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
        public async Task<Result<Product>> UpdateAsync(
            Guid productId, string? name = null, string? description = null,
            Guid? categoryId = null, Guid? measurementUnitId = null,
            CancellationToken cancellationToken = default)
        {
            var productDao = await _productRepository.GetByIdAsync(productId, cancellationToken);

            if (productDao == null)
                return Result<Product>.Failure("Товар/услуга не найдена");

            if (!string.IsNullOrWhiteSpace(name))
                productDao.Name = name;

            if (description != null)
                productDao.Description = description;

            if (categoryId.HasValue)
            {
                var categoryExists = await _categoryRepository.GetQueryable()
                    .AnyAsync(x => x.Id == categoryId.Value && x.IsActive, cancellationToken);

                if (!categoryExists)
                    return Result<Product>.Failure("Категория не найдена или неактивна");

                productDao.CategoryId = categoryId.Value;
            }

            if (measurementUnitId.HasValue)
            {
                var measurementUnitExists = await _measurementUnitRepository.GetQueryable()
                    .AnyAsync(x => x.Id == measurementUnitId.Value, cancellationToken);

                if (!measurementUnitExists)
                    return Result<Product>.Failure("Единица измерения не найдена");

                productDao.MeasurementUnitId = measurementUnitId.Value;
            }

            productDao.UpdatedAt = DateTime.UtcNow;

            _productRepository.Update(productDao);
            await SaveChangesAsync(cancellationToken);

            var product = _mapper.Map<Product>(productDao);
            return Result<Product>.Success(product);
        }

        /// <summary>
        /// Деактивировать товар/услугу
        /// </summary>
        /// <param name="productId">ID товара/услуги</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат операции</returns>
        public async Task<Result<bool>> DeactivateAsync(
            Guid productId, CancellationToken cancellationToken = default)
        {
            var productDao = await _productRepository.GetByIdAsync(productId, cancellationToken);

            if (productDao == null)
                return Result<bool>.Failure("Товар/услуга не найдена");

            productDao.IsActive = false;
            productDao.UpdatedAt = DateTime.UtcNow;

            _productRepository.Update(productDao);
            await SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }

        /// <summary>
        /// Активировать товар/услугу
        /// </summary>
        /// <param name="productId">ID товара/услуги</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат операции</returns>
        public async Task<Result<bool>> ActivateAsync(
            Guid productId, CancellationToken cancellationToken = default)
        {
            var productDao = await _productRepository.GetByIdAsync(productId, cancellationToken);

            if (productDao == null)
                return Result<bool>.Failure("Товар/услуга не найдена");

            productDao.IsActive = true;
            productDao.UpdatedAt = DateTime.UtcNow;

            _productRepository.Update(productDao);
            await SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }

        /// <summary>
        /// Удалить товар/услугу (только если нет связанных цен и элементов корзины)
        /// </summary>
        /// <param name="productId">ID товара/услуги</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат удаления</returns>
        public async Task<Result<bool>> DeleteAsync(
            Guid productId, CancellationToken cancellationToken = default)
        {
            var productDao = await _productRepository.GetByIdAsync(productId, cancellationToken);

            if (productDao == null)
                return Result<bool>.Failure("Товар/услуга не найдена");

            // Проверка на наличие цен
            var hasPrices = await _priceRepository.GetQueryable()
                .AnyAsync(x => x.ProductId == productId, cancellationToken);

            if (hasPrices)
                return Result<bool>.Failure("Невозможно удалить товар/услугу, для которой установлены цены");

            // Проверка на наличие в корзинах
            var hasCartItems = await _cartItemRepository.GetQueryable()
                .AnyAsync(x => x.ProductId == productId, cancellationToken);

            if (hasCartItems)
                return Result<bool>.Failure("Невозможно удалить товар/услугу, которая находится в корзинах пользователей");

            _productRepository.Delete(productDao);
            await SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
