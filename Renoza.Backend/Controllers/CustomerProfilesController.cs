using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Renoza.Domain.Entities.Profiles;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;

namespace Renoza.Backend.Controllers
{
    /// <summary>
    /// Контроллер управления профилями заказчиков
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CustomerProfilesController : ControllerBase
    {
        private readonly ILogger<CustomerProfilesController> _logger;
        private readonly ICustomerProfileService _customerProfileService;

        public CustomerProfilesController(
            ILogger<CustomerProfilesController> logger,
            ICustomerProfileService customerProfileService)
        {
            _logger = logger;
            _customerProfileService = customerProfileService;
        }

        /// <summary>
        /// Получить все профили заказчиков
        /// </summary>
        /// <returns>Список профилей заказчиков</returns>
        [HttpGet]
        public async Task<ActionResult> GetProfiles()
        {
            try
            {
                var profiles = await _customerProfileService.GetQueryable()
                    .Where(p => p.IsActive)
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();

                return Ok(profiles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка профилей заказчиков");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Произошла ошибка при обработке запроса" });
            }
        }

        /// <summary>
        /// Получить профиль заказчика по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор профиля</param>
        /// <returns>Профиль заказчика</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult> GetProfileById(Guid id)
        {
            try
            {
                var profile = await _customerProfileService.GetByIdAsync(id);

                if (profile == null)
                {
                    return NotFound(new { message = "Профиль не найден" });
                }

                return Ok(profile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении профиля заказчика {ProfileId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Произошла ошибка при обработке запроса" });
            }
        }

        /// <summary>
        /// Получить профиль заказчика по идентификатору пользователя
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns>Профиль заказчика</returns>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult> GetProfileByUserId(Guid userId)
        {
            try
            {
                var profile = await _customerProfileService.GetByUserIdAsync(userId);

                if (profile == null)
                {
                    return NotFound(new { message = "Профиль не найден" });
                }

                return Ok(profile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении профиля заказчика для пользователя {UserId}", userId);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Произошла ошибка при обработке запроса" });
            }
        }

        /// <summary>
        /// Создать профиль заказчика
        /// </summary>
        /// <param name="profile">Данные профиля</param>
        /// <returns>Созданный профиль</returns>
        [HttpPost]
        public async Task<ActionResult> CreateProfile([FromBody] CustomerProfile profile)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdProfile = await _customerProfileService.CreateAsync(profile);

                _logger.LogInformation("Создан профиль заказчика {ProfileId} для пользователя {UserId}",
                    createdProfile.Id, createdProfile.UserId);

                return CreatedAtAction(nameof(GetProfileById), new { id = createdProfile.Id }, createdProfile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании профиля заказчика");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Произошла ошибка при обработке запроса" });
            }
        }

        /// <summary>
        /// Обновить профиль заказчика
        /// </summary>
        /// <param name="id">Идентификатор профиля</param>
        /// <param name="profile">Обновленные данные профиля</param>
        /// <returns>Обновленный профиль</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProfile(Guid id, [FromBody] CustomerProfile profile)
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

                var updatedProfile = await _customerProfileService.UpdateAsync(profile);

                _logger.LogInformation("Обновлен профиль заказчика {ProfileId}", id);

                return Ok(updatedProfile);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Попытка обновления несуществующего профиля заказчика {ProfileId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении профиля заказчика {ProfileId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Произошла ошибка при обработке запроса" });
            }
        }

        /// <summary>
        /// Деактивировать профиль заказчика
        /// </summary>
        /// <param name="id">Идентификатор профиля</param>
        /// <returns>Результат операции</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeactivateProfile(Guid id)
        {
            try
            {
                var result = await _customerProfileService.DeactivateAsync(id);

                if (!result)
                {
                    return NotFound(new { message = "Профиль не найден" });
                }

                _logger.LogInformation("Деактивирован профиль заказчика {ProfileId}", id);
                return Ok(new { message = "Профиль успешно деактивирован" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при деактивации профиля заказчика {ProfileId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Произошла ошибка при обработке запроса" });
            }
        }
    }
}
