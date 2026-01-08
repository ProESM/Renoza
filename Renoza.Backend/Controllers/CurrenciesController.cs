using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;

namespace Renoza.Backend.Controllers
{
    /// <summary>
    /// Контроллер для работы со справочником валют
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CurrenciesController : ControllerBase
    {
        private readonly ILogger<CurrenciesController> _logger;
        private readonly ICurrencyService _currencyService;

        public CurrenciesController(
            ILogger<CurrenciesController> logger,
            ICurrencyService currencyService)
        {
            _logger = logger;
            _currencyService = currencyService;
        }

        /// <summary>
        /// Получить все валюты
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] bool includeInactive = false,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _currencyService.GetAllAsync(includeInactive, cancellationToken);

                if (!result.IsSuccess)
                {
                    return BadRequest(result.ErrorMessage);
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting currencies");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Получить валюту по ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(
            Guid id,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _currencyService.GetByIdAsync(id, cancellationToken);

                if (!result.IsSuccess)
                {
                    return NotFound(result.ErrorMessage);
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting currency by ID");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Получить валюту по коду
        /// </summary>
        [HttpGet("code/{code}")]
        public async Task<IActionResult> GetByCode(
            string code,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _currencyService.GetByCodeAsync(code, cancellationToken);

                if (!result.IsSuccess)
                {
                    return NotFound(result.ErrorMessage);
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting currency by code");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
