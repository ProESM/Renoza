using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;
using System.Security.Claims;

namespace Renoza.Backend.Controllers
{
    /// <summary>
    /// Контроллер управления избранным
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FavoritesController : ControllerBase
    {
        private readonly ILogger<FavoritesController> _logger;
        private readonly IFavoriteService _favoriteService;

        public FavoritesController(
            ILogger<FavoritesController> logger,
            IFavoriteService favoriteService)
        {
            _logger = logger;
            _favoriteService = favoriteService;
        }

        /// <summary>
        /// Получить все избранные профили текущего пользователя
        /// </summary>
        /// <returns>Список избранного</returns>
        [HttpGet]
        public async Task<ActionResult> GetMyFavorites()
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                var result = await _favoriteService.GetUserFavoritesAsync(userId);

                if (!result.IsSuccess)
                    return BadRequest(new { error = result.ErrorMessage });

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении избранного пользователя");
                return StatusCode(500, new { error = "Внутренняя ошибка сервера" });
            }
        }

        /// <summary>
        /// Добавить профиль в избранное
        /// </summary>
        /// <param name="profileId">ID профиля</param>
        /// <returns>Запись избранного</returns>
        [HttpPost("{profileId}")]
        public async Task<ActionResult> AddToFavorites(Guid profileId)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                var result = await _favoriteService.AddToFavoritesAsync(userId, profileId);

                if (!result.IsSuccess)
                    return BadRequest(new { error = result.ErrorMessage });

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при добавлении профиля в избранное. ProfileId: {ProfileId}", profileId);
                return StatusCode(500, new { error = "Внутренняя ошибка сервера" });
            }
        }

        /// <summary>
        /// Удалить профиль из избранного
        /// </summary>
        /// <param name="profileId">ID профиля</param>
        /// <returns>Результат удаления</returns>
        [HttpDelete("{profileId}")]
        public async Task<ActionResult> RemoveFromFavorites(Guid profileId)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                var result = await _favoriteService.RemoveFromFavoritesAsync(userId, profileId);

                if (!result.IsSuccess)
                    return BadRequest(new { error = result.ErrorMessage });

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении профиля из избранного. ProfileId: {ProfileId}", profileId);
                return StatusCode(500, new { error = "Внутренняя ошибка сервера" });
            }
        }

        /// <summary>
        /// Проверить, находится ли профиль в избранном
        /// </summary>
        /// <param name="profileId">ID профиля</param>
        /// <returns>True если в избранном, иначе False</returns>
        [HttpGet("check/{profileId}")]
        public async Task<ActionResult> IsFavorite(Guid profileId)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                var result = await _favoriteService.IsFavoriteAsync(userId, profileId);

                if (!result.IsSuccess)
                    return BadRequest(new { error = result.ErrorMessage });

                return Ok(new { isFavorite = result.Data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при проверке наличия профиля в избранном. ProfileId: {ProfileId}", profileId);
                return StatusCode(500, new { error = "Внутренняя ошибка сервера" });
            }
        }
    }
}
