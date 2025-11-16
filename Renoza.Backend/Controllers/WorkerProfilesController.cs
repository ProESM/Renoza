using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Renoza.Domain.Entities.Profiles;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;

namespace Renoza.Backend.Controllers
{
    /// <summary>
    /// Контроллер управления профилями работников
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WorkerProfilesController : ControllerBase
    {
        private readonly ILogger<WorkerProfilesController> _logger;
        private readonly IWorkerProfileService _workerProfileService;

        public WorkerProfilesController(
            ILogger<WorkerProfilesController> logger,
            IWorkerProfileService workerProfileService)
        {
            _logger = logger;
            _workerProfileService = workerProfileService;
        }

        /// <summary>
        /// Получить все профили работников
        /// </summary>
        /// <param name="isAvailable">Фильтр по доступности (необязательный)</param>
        /// <returns>Список профилей работников</returns>
        [HttpGet]
        public async Task<ActionResult> GetProfiles([FromQuery] bool? isAvailable = null)
        {
            try
            {
                var query = _workerProfileService.GetQueryable()
                    .Where(p => p.IsActive);

                if (isAvailable.HasValue)
                {
                    query = query.Where(p => p.IsAvailable == isAvailable.Value);
                }

                var profiles = await query
                    .OrderByDescending(p => p.Rating)
                    .ThenByDescending(p => p.CreatedAt)
                    .ToListAsync();

                return Ok(profiles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка профилей работников");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Произошла ошибка при обработке запроса" });
            }
        }

        /// <summary>
        /// Получить профиль работника по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор профиля</param>
        /// <returns>Профиль работника</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult> GetProfileById(Guid id)
        {
            try
            {
                var profile = await _workerProfileService.GetByIdAsync(id);

                if (profile == null)
                {
                    return NotFound(new { message = "Профиль не найден" });
                }

                return Ok(profile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении профиля работника {ProfileId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Произошла ошибка при обработке запроса" });
            }
        }

        /// <summary>
        /// Получить профиль работника по идентификатору пользователя
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns>Профиль работника</returns>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult> GetProfileByUserId(Guid userId)
        {
            try
            {
                var profile = await _workerProfileService.GetByUserIdAsync(userId);

                if (profile == null)
                {
                    return NotFound(new { message = "Профиль не найден" });
                }

                return Ok(profile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении профиля работника для пользователя {UserId}", userId);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Произошла ошибка при обработке запроса" });
            }
        }

        /// <summary>
        /// Создать профиль работника
        /// </summary>
        /// <param name="profile">Данные профиля</param>
        /// <returns>Созданный профиль</returns>
        [HttpPost]
        public async Task<ActionResult> CreateProfile([FromBody] WorkerProfile profile)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdProfile = await _workerProfileService.CreateAsync(profile);

                _logger.LogInformation("Создан профиль работника {ProfileId} для пользователя {UserId}",
                    createdProfile.Id, createdProfile.UserId);

                return CreatedAtAction(nameof(GetProfileById), new { id = createdProfile.Id }, createdProfile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании профиля работника");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Произошла ошибка при обработке запроса" });
            }
        }

        /// <summary>
        /// Обновить профиль работника
        /// </summary>
        /// <param name="id">Идентификатор профиля</param>
        /// <param name="profile">Обновленные данные профиля</param>
        /// <returns>Обновленный профиль</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProfile(Guid id, [FromBody] WorkerProfile profile)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (id != profile.Id)
                {
                    return BadRequest(new { message = "Идентификатор в URL не совпадает с идентификатором профиля" });
                }

                var updatedProfile = await _workerProfileService.UpdateAsync(profile);

                _logger.LogInformation("Обновлен профиль работника {ProfileId}", id);

                return Ok(updatedProfile);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Попытка обновления несуществующего профиля работника {ProfileId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении профиля работника {ProfileId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Произошла ошибка при обработке запроса" });
            }
        }

        /// <summary>
        /// Деактивировать профиль работника
        /// </summary>
        /// <param name="id">Идентификатор профиля</param>
        /// <returns>Результат операции</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeactivateProfile(Guid id)
        {
            try
            {
                var result = await _workerProfileService.DeactivateAsync(id);

                if (!result)
                {
                    return NotFound(new { message = "Профиль не найден" });
                }

                _logger.LogInformation("Деактивирован профиль работника {ProfileId}", id);
                return Ok(new { message = "Профиль успешно деактивирован" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при деактивации профиля работника {ProfileId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Произошла ошибка при обработке запроса" });
            }
        }

        /// <summary>
        /// Установить доступность работника
        /// </summary>
        /// <param name="id">Идентификатор профиля</param>
        /// <param name="isAvailable">Доступность</param>
        /// <returns>Результат операции</returns>
        [HttpPatch("{id}/availability")]
        public async Task<ActionResult> SetAvailability(Guid id, [FromBody] bool isAvailable)
        {
            try
            {
                var result = await _workerProfileService.SetAvailabilityAsync(id, isAvailable);

                if (!result)
                {
                    return NotFound(new { message = "Профиль не найден" });
                }

                _logger.LogInformation("Установлена доступность {IsAvailable} для профиля работника {ProfileId}",
                    isAvailable, id);

                return Ok(new { message = "Доступность успешно обновлена", isAvailable });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при установке доступности для профиля работника {ProfileId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Произошла ошибка при обработке запроса" });
            }
        }
    }
}
