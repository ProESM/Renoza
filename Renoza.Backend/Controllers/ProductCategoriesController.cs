using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Renoza.Backend.Models.ProductCategories;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;

namespace Renoza.Backend.Controllers
{
    /// <summary>
    /// Контроллер для работы с категориями товаров и услуг
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductCategoriesController : ControllerBase
    {
        private readonly ILogger<ProductCategoriesController> _logger;
        private readonly IProductCategoryService _productCategoryService;

        public ProductCategoriesController(
            ILogger<ProductCategoriesController> logger,
            IProductCategoryService productCategoryService)
        {
            _logger = logger;
            _productCategoryService = productCategoryService;
        }

        /// <summary>
        /// Создать новую категорию товаров/услуг
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateProductCategoryRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _productCategoryService.CreateAsync(
                    request.Name,
                    request.Description,
                    request.ParentId,
                    request.Order,
                    cancellationToken);

                if (!result.IsSuccess)
                {
                    return BadRequest(result.ErrorMessage);
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product category");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Получить категорию по ID
        /// </summary>
        [HttpGet("{categoryId}")]
        public async Task<IActionResult> GetById(
            Guid categoryId,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _productCategoryService.GetByIdAsync(categoryId, cancellationToken);

                if (!result.IsSuccess)
                {
                    return NotFound(result.ErrorMessage);
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product category by ID");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Получить все активные категории
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllActive(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _productCategoryService.GetAllActiveAsync(cancellationToken);

                if (!result.IsSuccess)
                {
                    return BadRequest(result.ErrorMessage);
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all active product categories");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Получить корневые категории (без родителей)
        /// </summary>
        [HttpGet("root")]
        public async Task<IActionResult> GetRootCategories(
            [FromQuery] bool includeInactive = false,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _productCategoryService.GetRootCategoriesAsync(includeInactive, cancellationToken);

                if (!result.IsSuccess)
                {
                    return BadRequest(result.ErrorMessage);
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting root product categories");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Получить дочерние категории для указанной родительской
        /// </summary>
        [HttpGet("{parentId}/children")]
        public async Task<IActionResult> GetChildCategories(
            Guid parentId,
            [FromQuery] bool includeInactive = false,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _productCategoryService.GetChildCategoriesAsync(parentId, includeInactive, cancellationToken);

                if (!result.IsSuccess)
                {
                    return BadRequest(result.ErrorMessage);
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting child product categories");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Обновить категорию
        /// </summary>
        [HttpPut("{categoryId}")]
        public async Task<IActionResult> Update(
            Guid categoryId,
            [FromBody] UpdateProductCategoryRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _productCategoryService.UpdateAsync(
                    categoryId,
                    request.Name,
                    request.Description,
                    request.Order,
                    cancellationToken);

                if (!result.IsSuccess)
                {
                    return BadRequest(result.ErrorMessage);
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product category");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Переместить категорию в другую родительскую категорию
        /// </summary>
        [HttpPut("{categoryId}/move")]
        public async Task<IActionResult> MoveToParent(
            Guid categoryId,
            [FromBody] MoveProductCategoryRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _productCategoryService.MoveToParentAsync(
                    categoryId,
                    request.NewParentId,
                    cancellationToken);

                if (!result.IsSuccess)
                {
                    return BadRequest(result.ErrorMessage);
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error moving product category");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Деактивировать категорию
        /// </summary>
        [HttpPut("{categoryId}/deactivate")]
        public async Task<IActionResult> Deactivate(
            Guid categoryId,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _productCategoryService.DeactivateAsync(categoryId, cancellationToken);

                if (!result.IsSuccess)
                {
                    return BadRequest(result.ErrorMessage);
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating product category");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Активировать категорию
        /// </summary>
        [HttpPut("{categoryId}/activate")]
        public async Task<IActionResult> Activate(
            Guid categoryId,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _productCategoryService.ActivateAsync(categoryId, cancellationToken);

                if (!result.IsSuccess)
                {
                    return BadRequest(result.ErrorMessage);
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating product category");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Удалить категорию (только если в ней нет товаров и дочерних категорий)
        /// </summary>
        [HttpDelete("{categoryId}")]
        public async Task<IActionResult> Delete(
            Guid categoryId,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _productCategoryService.DeleteAsync(categoryId, cancellationToken);

                if (!result.IsSuccess)
                {
                    return BadRequest(result.ErrorMessage);
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product category");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
