using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;

namespace Renoza.Backend.Controllers
{
    /// <summary>
    /// Контроллер управления ролями
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RolesController : ControllerBase
    {
        private readonly ILogger<RolesController> _logger;
        private readonly IRoleService _roleService;

        public RolesController(
            ILogger<RolesController> logger,
            IRoleService roleService)
        {
            _logger = logger;
            _roleService = roleService;
        }

        /// <summary>
        /// Получить все роли
        /// </summary>
        /// <returns>Список ролей</returns>
        [HttpGet]
        public async Task<ActionResult> GetRoles()
        {
            try
            {
                var roles = await _roleService.GetQueryable()
                    .Where(r => r.IsActive)
                    .OrderBy(r => r.Name)
                    .ToListAsync();

                return Ok(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка ролей");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Произошла ошибка при обработке запроса" });
            }
        }

        /// <summary>
        /// Получить роль по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор роли</param>
        /// <returns>Роль</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult> GetRoleById(Guid id)
        {
            try
            {
                var role = await _roleService.GetByIdAsync(id);

                if (role == null)
                {
                    return NotFound(new { message = "Роль не найдена" });
                }

                return Ok(role);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении роли {RoleId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Произошла ошибка при обработке запроса" });
            }
        }

        /// <summary>
        /// Получить роли пользователя
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns>Список ролей пользователя</returns>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult> GetUserRoles(Guid userId)
        {
            try
            {
                var roles = await _roleService.GetUserRolesAsync(userId);
                return Ok(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении ролей пользователя {UserId}", userId);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Произошла ошибка при обработке запроса" });
            }
        }

        /// <summary>
        /// Назначить роль пользователю
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="roleId">Идентификатор роли</param>
        /// <returns>Результат операции</returns>
        [HttpPost("user/{userId}/role/{roleId}")]
        public async Task<ActionResult> AssignRoleToUser(Guid userId, Guid roleId)
        {
            try
            {
                var result = await _roleService.AssignRoleToUserAsync(userId, roleId);

                if (!result)
                {
                    return BadRequest(new { message = "Не удалось назначить роль" });
                }

                _logger.LogInformation("Роль {RoleId} назначена пользователю {UserId}", roleId, userId);
                return Ok(new { message = "Роль успешно назначена" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при назначении роли {RoleId} пользователю {UserId}", roleId, userId);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Произошла ошибка при обработке запроса" });
            }
        }

        /// <summary>
        /// Отозвать роль у пользователя
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="roleId">Идентификатор роли</param>
        /// <returns>Результат операции</returns>
        [HttpDelete("user/{userId}/role/{roleId}")]
        public async Task<ActionResult> RevokeRoleFromUser(Guid userId, Guid roleId)
        {
            try
            {
                var result = await _roleService.RevokeRoleFromUserAsync(userId, roleId);

                if (!result)
                {
                    return NotFound(new { message = "Роль не найдена или уже отозвана" });
                }

                _logger.LogInformation("Роль {RoleId} отозвана у пользователя {UserId}", roleId, userId);
                return Ok(new { message = "Роль успешно отозвана" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при отзыве роли {RoleId} у пользователя {UserId}", roleId, userId);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Произошла ошибка при обработке запроса" });
            }
        }
    }
}
