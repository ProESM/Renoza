using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Renoza.Backend.Models.Cart;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;
using System.Security.Claims;

namespace Renoza.Backend.Controllers
{
    /// <summary>
    /// Контроллер для работы с корзиной покупок
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ILogger<CartController> _logger;
        private readonly ICartItemService _cartItemService;

        public CartController(
            ILogger<CartController> logger,
            ICartItemService cartItemService)
        {
            _logger = logger;
            _cartItemService = cartItemService;
        }

        /// <summary>
        /// Получить элементы корзины текущего пользователя
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetCartItems(CancellationToken cancellationToken)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found in token");
                }

                var result = await _cartItemService.GetCartItemsAsync(Guid.Parse(userId), cancellationToken);

                if (!result.IsSuccess)
                {
                    return BadRequest(result.ErrorMessage);
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cart items");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Добавить товар в корзину
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddToCart(
            [FromBody] AddToCartRequest request,
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

                var result = await _cartItemService.AddToCartAsync(
                    Guid.Parse(userId),
                    request.ProfileId,
                    request.ProductId,
                    request.Quantity,
                    request.Notes,
                    cancellationToken);

                if (!result.IsSuccess)
                {
                    return BadRequest(result.ErrorMessage);
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding to cart");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Обновить количество товара в корзине
        /// </summary>
        [HttpPut("{cartItemId}")]
        public async Task<IActionResult> UpdateQuantity(
            Guid cartItemId,
            [FromBody] UpdateQuantityRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _cartItemService.UpdateQuantityAsync(
                    cartItemId,
                    request.Quantity,
                    cancellationToken);

                if (!result.IsSuccess)
                {
                    return BadRequest(result.ErrorMessage);
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cart item quantity");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Удалить товар из корзины
        /// </summary>
        [HttpDelete("{cartItemId}")]
        public async Task<IActionResult> RemoveFromCart(
            Guid cartItemId,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _cartItemService.RemoveFromCartAsync(cartItemId, cancellationToken);

                if (!result.IsSuccess)
                {
                    return BadRequest(result.ErrorMessage);
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cart item");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Очистить корзину текущего пользователя
        /// </summary>
        [HttpDelete]
        public async Task<IActionResult> ClearCart(CancellationToken cancellationToken)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found in token");
                }

                var result = await _cartItemService.ClearCartAsync(Guid.Parse(userId), cancellationToken);

                if (!result.IsSuccess)
                {
                    return BadRequest(result.ErrorMessage);
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cart");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
