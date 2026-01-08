using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Renoza.Backend.Models.ProfileProductPrices;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;
using System.Security.Claims;

namespace Renoza.Backend.Controllers
{
    /// <summary>
    /// Контроллер для управления ценами профилей на товары/услуги
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProfileProductPricesController : ControllerBase
    {
        private readonly ILogger<ProfileProductPricesController> _logger;
        private readonly IProfileProductPriceService _profileProductPriceService;

        public ProfileProductPricesController(
            ILogger<ProfileProductPricesController> logger,
            IProfileProductPriceService profileProductPriceService)
        {
            _logger = logger;
            _profileProductPriceService = profileProductPriceService;
        }

        /// <summary>
        /// Установить цену для профиля на товар/услугу (требуется роль Owner или Manager)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> SetPrice(
            [FromBody] SetPriceRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found in token");
                }

                var result = await _profileProductPriceService.SetPriceAsync(
                    Guid.Parse(userId),
                    request.ProfileId,
                    request.ProductId,
                    request.CurrencyId,
                    request.Price,
                    request.StartDate,
                    request.EndDate,
                    cancellationToken);

                if (!result.IsSuccess)
                {
                    return BadRequest(result.ErrorMessage);
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting price");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Получить текущую цену для профиля на товар/услугу
        /// </summary>
        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentPrice(
            [FromQuery] Guid profileId,
            [FromQuery] Guid productId,
            [FromQuery] DateTime? date = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _profileProductPriceService.GetCurrentPriceAsync(
                    profileId,
                    productId,
                    date,
                    cancellationToken);

                if (!result.IsSuccess)
                {
                    return NotFound(result.ErrorMessage);
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current price");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Получить все цены для профиля
        /// </summary>
        [HttpGet("profile/{profileId}")]
        public async Task<IActionResult> GetPricesByProfile(
            Guid profileId,
            [FromQuery] bool includeExpired = false,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _profileProductPriceService.GetPricesByProfileAsync(
                    profileId,
                    includeExpired,
                    cancellationToken);

                if (!result.IsSuccess)
                {
                    return BadRequest(result.ErrorMessage);
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting prices by profile");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Получить историю цен для профиля на конкретный товар/услугу
        /// </summary>
        [HttpGet("history")]
        public async Task<IActionResult> GetPriceHistory(
            [FromQuery] Guid profileId,
            [FromQuery] Guid productId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _profileProductPriceService.GetPriceHistoryAsync(
                    profileId,
                    productId,
                    cancellationToken);

                if (!result.IsSuccess)
                {
                    return BadRequest(result.ErrorMessage);
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting price history");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Обновить цену для профиля на товар/услугу (требуется роль Owner или Manager)
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> UpdatePrice(
            [FromBody] UpdatePriceRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found in token");
                }

                var result = await _profileProductPriceService.UpdatePriceAsync(
                    Guid.Parse(userId),
                    request.ProfileId,
                    request.ProductId,
                    request.OldStartDate,
                    request.NewPrice,
                    request.NewCurrencyId,
                    request.NewEndDate,
                    cancellationToken);

                if (!result.IsSuccess)
                {
                    return BadRequest(result.ErrorMessage);
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating price");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Завершить действие цены (установить EndDate на текущую дату) (требуется роль Owner или Manager)
        /// </summary>
        [HttpPut("terminate")]
        public async Task<IActionResult> TerminatePrice(
            [FromQuery] Guid profileId,
            [FromQuery] Guid productId,
            [FromQuery] DateTime startDate,
            CancellationToken cancellationToken)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found in token");
                }

                var result = await _profileProductPriceService.TerminatePriceAsync(
                    Guid.Parse(userId),
                    profileId,
                    productId,
                    startDate,
                    cancellationToken);

                if (!result.IsSuccess)
                {
                    return BadRequest(result.ErrorMessage);
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error terminating price");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Удалить цену для профиля на товар/услугу (требуется роль Owner или Manager)
        /// </summary>
        [HttpDelete]
        public async Task<IActionResult> DeletePrice(
            [FromQuery] Guid profileId,
            [FromQuery] Guid productId,
            [FromQuery] DateTime startDate,
            CancellationToken cancellationToken)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found in token");
                }

                var result = await _profileProductPriceService.DeletePriceAsync(
                    Guid.Parse(userId),
                    profileId,
                    productId,
                    startDate,
                    cancellationToken);

                if (!result.IsSuccess)
                {
                    return BadRequest(result.ErrorMessage);
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting price");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
