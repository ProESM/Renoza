using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;

namespace Renoza.Backend.Controllers
{
    /// <summary>
    /// Контроллер для работы со справочником ролей участников компаний
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MemberRolesController : ControllerBase
    {
        private readonly ILogger<MemberRolesController> _logger;
        private readonly IMemberRoleService _memberRoleService;

        public MemberRolesController(
            ILogger<MemberRolesController> logger,
            IMemberRoleService memberRoleService)
        {
            _logger = logger;
            _memberRoleService = memberRoleService;
        }

        /// <summary>
        /// Получить все роли участников компаний
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] bool includeInactive = false,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _memberRoleService.GetAllAsync(includeInactive, cancellationToken);

                if (!result.IsSuccess)
                {
                    return BadRequest(result.ErrorMessage);
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting member roles");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Получить роль по ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(
            Guid id,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _memberRoleService.GetByIdAsync(id, cancellationToken);

                if (!result.IsSuccess)
                {
                    return NotFound(result.ErrorMessage);
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting member role by ID");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Получить роль по коду
        /// </summary>
        [HttpGet("code/{code}")]
        public async Task<IActionResult> GetByCode(
            string code,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _memberRoleService.GetByCodeAsync(code, cancellationToken);

                if (!result.IsSuccess)
                {
                    return NotFound(result.ErrorMessage);
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting member role by code");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
