using Renoza.Common.Base.Helpers;
using Renoza.Domain.Entities.Favorites;

namespace Renoza.Domain.Services.Interfaces.RenozaInterfaces
{
    /// <summary>
    /// Сервис для управления избранным
    /// </summary>
    public interface IFavoriteService
    {
        /// <summary>
        /// Добавить профиль в избранное
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <param name="profileId">ID профиля</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Запись избранного</returns>
        Task<Result<Favorite>> AddToFavoritesAsync(Guid userId, Guid profileId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Удалить профиль из избранного
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <param name="profileId">ID профиля</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат удаления</returns>
        Task<Result<bool>> RemoveFromFavoritesAsync(Guid userId, Guid profileId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить все избранные профили пользователя
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список избранного</returns>
        Task<Result<List<Favorite>>> GetUserFavoritesAsync(Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Проверить, находится ли профиль в избранном у пользователя
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <param name="profileId">ID профиля</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>True если в избранном, иначе False</returns>
        Task<Result<bool>> IsFavoriteAsync(Guid userId, Guid profileId, CancellationToken cancellationToken = default);
    }
}
