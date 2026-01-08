using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Renoza.Common.Base.Helpers;
using Renoza.Domain.Entities.Favorites;
using Renoza.Domain.Services.Implementations.BaseImplementations;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;
using Renoza.Infrastructure.Contexts;
using Renoza.Infrastructure.Entities.Renoza;
using Renoza.Infrastructure.Repositories.Interfaces.BaseInterfaces;

namespace Renoza.Domain.Services.Implementations.RenozaImplementations
{
    /// <summary>
    /// Сервис для управления избранным
    /// </summary>
    public class FavoriteService : BaseService<RenozaContext>, IFavoriteService
    {
        /// <summary>
        /// Репозиторий для работы с избранным
        /// </summary>
        private readonly IEntityWithIdRepository<FavoriteDao, Guid> _favoriteRepository;

        /// <summary>
        /// Маппер для преобразования между DAO и Domain сущностями
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Конструктор сервиса для работы с избранным
        /// </summary>
        /// <param name="dbContext">Контекст базы данных</param>
        /// <param name="favoriteRepository">Репозиторий для работы с избранным</param>
        /// <param name="mapper">Маппер для преобразования между DAO и Domain сущностями</param>
        public FavoriteService(
            RenozaContext dbContext,
            IEntityWithIdRepository<FavoriteDao, Guid> favoriteRepository,
            IMapper mapper) : base(dbContext)
        {
            _favoriteRepository = favoriteRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Добавить профиль в избранное
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <param name="profileId">ID профиля</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Запись избранного</returns>
        public async Task<Result<Favorite>> AddToFavoritesAsync(
            Guid userId,
            Guid profileId,
            CancellationToken cancellationToken = default)
        {
            // Проверяем, нет ли уже такой записи в избранном
            var existingFavoriteDao = await _favoriteRepository.GetQueryable()
                .FirstOrDefaultAsync(f => f.UserId == userId && f.ProfileId == profileId, cancellationToken);

            if (existingFavoriteDao != null)
                return Result<Favorite>.Failure("Профиль уже находится в избранном");

            // Создаем новую запись в избранном
            var favoriteDao = new FavoriteDao
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ProfileId = profileId,
                CreatedAt = DateTime.UtcNow
            };

            await _favoriteRepository.CreateAsync(favoriteDao, cancellationToken);
            await SaveChangesAsync(cancellationToken);

            var favorite = _mapper.Map<Favorite>(favoriteDao);
            return Result<Favorite>.Success(favorite);
        }

        /// <summary>
        /// Удалить профиль из избранного
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <param name="profileId">ID профиля</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат удаления</returns>
        public async Task<Result<bool>> RemoveFromFavoritesAsync(
            Guid userId,
            Guid profileId,
            CancellationToken cancellationToken = default)
        {
            var favoriteDao = await _favoriteRepository.GetQueryable()
                .FirstOrDefaultAsync(f => f.UserId == userId && f.ProfileId == profileId, cancellationToken);

            if (favoriteDao == null)
                return Result<bool>.Failure("Профиль не найден в избранном");

            _favoriteRepository.Delete(favoriteDao);
            await SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }

        /// <summary>
        /// Получить все избранные профили пользователя
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список избранного</returns>
        public async Task<Result<List<Favorite>>> GetUserFavoritesAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var favoriteDaos = await _favoriteRepository.GetQueryable()
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync(cancellationToken);

            var favorites = _mapper.Map<List<Favorite>>(favoriteDaos);
            return Result<List<Favorite>>.Success(favorites);
        }

        /// <summary>
        /// Проверить, находится ли профиль в избранном у пользователя
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <param name="profileId">ID профиля</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>True если в избранном, иначе False</returns>
        public async Task<Result<bool>> IsFavoriteAsync(
            Guid userId,
            Guid profileId,
            CancellationToken cancellationToken = default)
        {
            var exists = await _favoriteRepository.GetQueryable()
                .AnyAsync(f => f.UserId == userId && f.ProfileId == profileId, cancellationToken);

            return Result<bool>.Success(exists);
        }
    }
}
