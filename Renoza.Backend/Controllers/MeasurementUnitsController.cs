using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;

namespace Renoza.Backend.Controllers
{
    /// <summary>
    /// Контроллер для работы со справочником единиц измерения
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MeasurementUnitsController : ControllerBase
    {
        private readonly ILogger<MeasurementUnitsController> _logger;
        private readonly IMeasurementUnitService _measurementUnitService;

        public MeasurementUnitsController(
            ILogger<MeasurementUnitsController> logger,
            IMeasurementUnitService measurementUnitService)
        {
            _logger = logger;
            _measurementUnitService = measurementUnitService;
        }

        /// <summary>
        /// Получить все единицы измерения
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] bool includeInactive = false,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _measurementUnitService.GetAllAsync(includeInactive, cancellationToken);

                if (!result.IsSuccess)
                {
                    return BadRequest(result.ErrorMessage);
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting measurement units");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Получить единицу измерения по ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(
            Guid id,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _measurementUnitService.GetByIdAsync(id, cancellationToken);

                if (!result.IsSuccess)
                {
                    return NotFound(result.ErrorMessage);
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting measurement unit by ID");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Получить единицу измерения по коду
        /// </summary>
        [HttpGet("code/{code}")]
        public async Task<IActionResult> GetByCode(
            string code,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _measurementUnitService.GetByCodeAsync(code, cancellationToken);

                if (!result.IsSuccess)
                {
                    return NotFound(result.ErrorMessage);
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting measurement unit by code");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Получить единицы измерения по типу
        /// </summary>
        [HttpGet("type/{typeId}")]
        public async Task<IActionResult> GetByType(
            Guid typeId,
            [FromQuery] bool includeInactive = false,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _measurementUnitService.GetByTypeAsync(typeId, includeInactive, cancellationToken);

                if (!result.IsSuccess)
                {
                    return BadRequest(result.ErrorMessage);
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting measurement units by type");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
