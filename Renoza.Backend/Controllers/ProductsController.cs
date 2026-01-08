using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Renoza.Backend.Models.Products;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;

namespace Renoza.Backend.Controllers
{
    /// <summary>
    /// Контроллер для работы с товарами и услугами
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly IProductService _productService;

        public ProductsController(
            ILogger<ProductsController> logger,
            IProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }

        /// <summary>
        /// Создать новый товар/услугу
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateProductRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _productService.CreateAsync(
                    request.CategoryId,
                    request.Name,
                    request.MeasurementUnitId,
                    request.Description,
                    cancellationToken);

                if (!result.IsSuccess)
                {
                    return BadRequest(result.ErrorMessage);
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Получить товар/услугу по ID
        /// </summary>
        [HttpGet("{productId}")]
        public async Task<IActionResult> GetById(
            Guid productId,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _productService.GetByIdAsync(productId, cancellationToken);

                if (!result.IsSuccess)
                {
                    return NotFound(result.ErrorMessage);
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product by ID");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Получить все активные товары/услуги
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllActive(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _productService.GetAllActiveAsync(cancellationToken);

                if (!result.IsSuccess)
                {
                    return BadRequest(result.ErrorMessage);
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all active products");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Получить товары/услуги по категории
        /// </summary>
        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetByCategory(
            Guid categoryId,
            [FromQuery] bool includeInactive = false,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _productService.GetByCategoryAsync(categoryId, includeInactive, cancellationToken);

                if (!result.IsSuccess)
                {
                    return BadRequest(result.ErrorMessage);
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products by category");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Поиск товаров/услуг по названию
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> SearchByName(
            [FromQuery] string searchTerm,
            [FromQuery] bool includeInactive = false,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _productService.SearchByNameAsync(searchTerm, includeInactive, cancellationToken);

                if (!result.IsSuccess)
                {
                    return BadRequest(result.ErrorMessage);
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching products by name");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Обновить товар/услугу
        /// </summary>
        [HttpPut("{productId}")]
        public async Task<IActionResult> Update(
            Guid productId,
            [FromBody] UpdateProductRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _productService.UpdateAsync(
                    productId,
                    request.Name,
                    request.Description,
                    request.CategoryId,
                    request.MeasurementUnitId,
                    cancellationToken);

                if (!result.IsSuccess)
                {
                    return BadRequest(result.ErrorMessage);
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Деактивировать товар/услугу
        /// </summary>
        [HttpPut("{productId}/deactivate")]
        public async Task<IActionResult> Deactivate(
            Guid productId,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _productService.DeactivateAsync(productId, cancellationToken);

                if (!result.IsSuccess)
                {
                    return BadRequest(result.ErrorMessage);
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating product");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Активировать товар/услугу
        /// </summary>
        [HttpPut("{productId}/activate")]
        public async Task<IActionResult> Activate(
            Guid productId,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _productService.ActivateAsync(productId, cancellationToken);

                if (!result.IsSuccess)
                {
                    return BadRequest(result.ErrorMessage);
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating product");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Удалить товар/услугу (только если нет связанных цен и элементов корзины)
        /// </summary>
        [HttpDelete("{productId}")]
        public async Task<IActionResult> Delete(
            Guid productId,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _productService.DeleteAsync(productId, cancellationToken);

                if (!result.IsSuccess)
                {
                    return BadRequest(result.ErrorMessage);
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
