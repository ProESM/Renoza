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
    /// Сервис для управления категориями товаров и услуг
    /// </summary>
    public class ProductCategoryService : BaseService<RenozaContext>, IProductCategoryService
    {
        /// <summary>
        /// Репозиторий для работы с категориями товаров
        /// </summary>
        private readonly IEntityWithIdRepository<ProductCategoryDao, Guid> _categoryRepository;

        /// <summary>
        /// Репозиторий для работы с товарами
        /// </summary>
        private readonly IEntityWithIdRepository<ProductDao, Guid> _productRepository;

        /// <summary>
        /// Маппер для преобразования между DAO и Domain сущностями
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Сервис для управления категориями товаров и услуг
        /// </summary>
        /// <param name="dbContext">Контекст базы данных</param>
        /// <param name="categoryRepository">Репозиторий для работы с категориями товаров</param>
        /// <param name="productRepository">Репозиторий для работы с товарами</param>
        /// <param name="mapper">Маппер для преобразования между DAO и Domain сущностями</param>
        public ProductCategoryService(
            RenozaContext dbContext,
            IEntityWithIdRepository<ProductCategoryDao, Guid> categoryRepository,
            IEntityWithIdRepository<ProductDao, Guid> productRepository,
            IMapper mapper) : base(dbContext)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Создать новую категорию
        /// </summary>
        /// <param name="name">Название категории</param>
        /// <param name="description">Описание категории</param>
        /// <param name="parentId">ID родительской категории (null для корневой)</param>
        /// <param name="order">Порядок сортировки</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Созданная категория</returns>
        public async Task<Result<ProductCategory>> CreateAsync(
            string name, string? description = null, Guid? parentId = null, int order = 0,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result<ProductCategory>.Failure("Название категории не может быть пустым");

            // Проверка существования родительской категории
            if (parentId.HasValue)
            {
                var parentExists = await _categoryRepository.GetQueryable()
                    .AnyAsync(x => x.Id == parentId.Value, cancellationToken);

                if (!parentExists)
                    return Result<ProductCategory>.Failure("Родительская категория не найдена");
            }

            var categoryDao = new ProductCategoryDao
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = description,
                ParentId = parentId,
                Order = order,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _categoryRepository.CreateAsync(categoryDao, cancellationToken);
            await SaveChangesAsync(cancellationToken);

            var category = _mapper.Map<ProductCategory>(categoryDao);
            return Result<ProductCategory>.Success(category);
        }

        /// <summary>
        /// Получить категорию по ID
        /// </summary>
        /// <param name="categoryId">ID категории</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Категория</returns>
        public async Task<Result<ProductCategory>> GetByIdAsync(
            Guid categoryId, CancellationToken cancellationToken = default)
        {
            var categoryDao = await _categoryRepository.GetByIdAsync(categoryId, cancellationToken);

            if (categoryDao == null)
                return Result<ProductCategory>.Failure("Категория не найдена");

            var category = _mapper.Map<ProductCategory>(categoryDao);
            return Result<ProductCategory>.Success(category);
        }

        /// <summary>
        /// Получить все активные категории
        /// </summary>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список активных категорий</returns>
        public async Task<Result<List<ProductCategory>>> GetAllActiveAsync(
            CancellationToken cancellationToken = default)
        {
            var categoriesDaos = await _categoryRepository.GetQueryable()
                .Where(x => x.IsActive)
                .OrderBy(x => x.ParentId)
                .ThenBy(x => x.Order)
                .ThenBy(x => x.Name)
                .ToListAsync(cancellationToken);

            var categories = _mapper.Map<List<ProductCategory>>(categoriesDaos);
            return Result<List<ProductCategory>>.Success(categories);
        }

        /// <summary>
        /// Получить корневые категории (без родителей)
        /// </summary>
        /// <param name="includeInactive">Включить неактивные категории</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список корневых категорий</returns>
        public async Task<Result<List<ProductCategory>>> GetRootCategoriesAsync(
            bool includeInactive = false, CancellationToken cancellationToken = default)
        {
            var query = _categoryRepository.GetQueryable()
                .Where(x => x.ParentId == null);

            if (!includeInactive)
                query = query.Where(x => x.IsActive);

            var categoriesDaos = await query
                .OrderBy(x => x.Order)
                .ThenBy(x => x.Name)
                .ToListAsync(cancellationToken);

            var categories = _mapper.Map<List<ProductCategory>>(categoriesDaos);
            return Result<List<ProductCategory>>.Success(categories);
        }

        /// <summary>
        /// Получить дочерние категории для указанной родительской
        /// </summary>
        /// <param name="parentId">ID родительской категории</param>
        /// <param name="includeInactive">Включить неактивные категории</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список дочерних категорий</returns>
        public async Task<Result<List<ProductCategory>>> GetChildCategoriesAsync(
            Guid parentId, bool includeInactive = false, CancellationToken cancellationToken = default)
        {
            var query = _categoryRepository.GetQueryable()
                .Where(x => x.ParentId == parentId);

            if (!includeInactive)
                query = query.Where(x => x.IsActive);

            var categoriesDaos = await query
                .OrderBy(x => x.Order)
                .ThenBy(x => x.Name)
                .ToListAsync(cancellationToken);

            var categories = _mapper.Map<List<ProductCategory>>(categoriesDaos);
            return Result<List<ProductCategory>>.Success(categories);
        }

        /// <summary>
        /// Обновить категорию
        /// </summary>
        /// <param name="categoryId">ID категории</param>
        /// <param name="name">Новое название</param>
        /// <param name="description">Новое описание</param>
        /// <param name="order">Новый порядок сортировки</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Обновленная категория</returns>
        public async Task<Result<ProductCategory>> UpdateAsync(
            Guid categoryId, string? name = null, string? description = null, int? order = null,
            CancellationToken cancellationToken = default)
        {
            var categoryDao = await _categoryRepository.GetByIdAsync(categoryId, cancellationToken);

            if (categoryDao == null)
                return Result<ProductCategory>.Failure("Категория не найдена");

            if (!string.IsNullOrWhiteSpace(name))
                categoryDao.Name = name;

            if (description != null)
                categoryDao.Description = description;

            if (order.HasValue)
                categoryDao.Order = order.Value;

            categoryDao.UpdatedAt = DateTime.UtcNow;

            _categoryRepository.Update(categoryDao);
            await SaveChangesAsync(cancellationToken);

            var category = _mapper.Map<ProductCategory>(categoryDao);
            return Result<ProductCategory>.Success(category);
        }

        /// <summary>
        /// Переместить категорию в другую родительскую категорию
        /// </summary>
        /// <param name="categoryId">ID категории для перемещения</param>
        /// <param name="newParentId">ID новой родительской категории (null для перемещения в корень)</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Обновленная категория</returns>
        public async Task<Result<ProductCategory>> MoveToParentAsync(
            Guid categoryId, Guid? newParentId, CancellationToken cancellationToken = default)
        {
            var categoryDao = await _categoryRepository.GetByIdAsync(categoryId, cancellationToken);

            if (categoryDao == null)
                return Result<ProductCategory>.Failure("Категория не найдена");

            // Проверка на циклическую зависимость
            if (newParentId.HasValue)
            {
                if (newParentId == categoryId)
                    return Result<ProductCategory>.Failure("Категория не может быть родителем самой себе");

                // Проверка, что новый родитель не является потомком текущей категории
                var isDescendant = await IsDescendantOfAsync(newParentId.Value, categoryId, cancellationToken);
                if (isDescendant)
                    return Result<ProductCategory>.Failure("Невозможно переместить категорию в свою подкатегорию");

                var parentExists = await _categoryRepository.GetQueryable()
                    .AnyAsync(x => x.Id == newParentId.Value, cancellationToken);

                if (!parentExists)
                    return Result<ProductCategory>.Failure("Родительская категория не найдена");
            }

            categoryDao.ParentId = newParentId;
            categoryDao.UpdatedAt = DateTime.UtcNow;

            _categoryRepository.Update(categoryDao);
            await SaveChangesAsync(cancellationToken);

            var category = _mapper.Map<ProductCategory>(categoryDao);
            return Result<ProductCategory>.Success(category);
        }

        /// <summary>
        /// Деактивировать категорию
        /// </summary>
        /// <param name="categoryId">ID категории</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат операции</returns>
        public async Task<Result<bool>> DeactivateAsync(
            Guid categoryId, CancellationToken cancellationToken = default)
        {
            var categoryDao = await _categoryRepository.GetByIdAsync(categoryId, cancellationToken);

            if (categoryDao == null)
                return Result<bool>.Failure("Категория не найдена");

            categoryDao.IsActive = false;
            categoryDao.UpdatedAt = DateTime.UtcNow;

            _categoryRepository.Update(categoryDao);
            await SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }

        /// <summary>
        /// Активировать категорию
        /// </summary>
        /// <param name="categoryId">ID категории</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат операции</returns>
        public async Task<Result<bool>> ActivateAsync(
            Guid categoryId, CancellationToken cancellationToken = default)
        {
            var categoryDao = await _categoryRepository.GetByIdAsync(categoryId, cancellationToken);

            if (categoryDao == null)
                return Result<bool>.Failure("Категория не найдена");

            categoryDao.IsActive = true;
            categoryDao.UpdatedAt = DateTime.UtcNow;

            _categoryRepository.Update(categoryDao);
            await SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }

        /// <summary>
        /// Удалить категорию (только если в ней нет товаров и дочерних категорий)
        /// </summary>
        /// <param name="categoryId">ID категории</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат удаления</returns>
        public async Task<Result<bool>> DeleteAsync(
            Guid categoryId, CancellationToken cancellationToken = default)
        {
            var categoryDao = await _categoryRepository.GetByIdAsync(categoryId, cancellationToken);

            if (categoryDao == null)
                return Result<bool>.Failure("Категория не найдена");

            // Проверка на наличие дочерних категорий
            var hasChildren = await _categoryRepository.GetQueryable()
                .AnyAsync(x => x.ParentId == categoryId, cancellationToken);

            if (hasChildren)
                return Result<bool>.Failure("Невозможно удалить категорию, содержащую подкатегории");

            // Проверка на наличие товаров в категории
            var hasProducts = await _productRepository.GetQueryable()
                .AnyAsync(x => x.CategoryId == categoryId, cancellationToken);

            if (hasProducts)
                return Result<bool>.Failure("Невозможно удалить категорию, содержащую товары");

            _categoryRepository.Delete(categoryDao);
            await SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }

        /// <summary>
        /// Проверить, является ли категория потомком другой категории
        /// </summary>
        /// <param name="categoryId">ID проверяемой категории</param>
        /// <param name="ancestorId">ID предполагаемого предка</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>True если categoryId является потомком ancestorId</returns>
        private async Task<bool> IsDescendantOfAsync(
            Guid categoryId, Guid ancestorId, CancellationToken cancellationToken)
        {
            var currentId = categoryId;

            while (currentId != Guid.Empty)
            {
                var categoryDao = await _categoryRepository.GetByIdAsync(currentId, cancellationToken);

                if (categoryDao == null || !categoryDao.ParentId.HasValue)
                    return false;

                if (categoryDao.ParentId.Value == ancestorId)
                    return true;

                currentId = categoryDao.ParentId.Value;
            }

            return false;
        }
    }
}
