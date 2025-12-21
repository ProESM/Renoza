using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Renoza.Backend.Models.Documents;
using Renoza.Domain.Entities.Documents;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;
using System.Security.Claims;

namespace Renoza.Backend.Controllers
{
    /// <summary>
    /// Контроллер для работы с документами
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentService _documentService;
        private readonly ILogger<DocumentsController> _logger;

        public DocumentsController(
            IDocumentService documentService,
            ILogger<DocumentsController> logger)
        {
            _documentService = documentService;
            _logger = logger;
        }

        /// <summary>
        /// Создать документ из шаблона
        /// </summary>
        /// <param name="request">Данные для создания документа</param>
        /// <returns>Результат создания</returns>
        [HttpPost]
        [ProducesResponseType(typeof(DocumentGenerationResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateDocumentRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == Guid.Empty)
                {
                    return Unauthorized(new { message = "Пользователь не авторизован" });
                }

                var input = new GenerateDocumentInput
                {
                    TemplateId = request.TemplateId,
                    FormatId = request.FormatId,
                    Name = request.Name,
                    PlaceholderData = request.PlaceholderData,
                    OrderId = request.OrderId,
                    CustomerId = request.CustomerId,
                    CreatedBy = userId
                };

                var result = await _documentService.CreateAsync(input);

                if (!result.IsSuccess)
                {
                    return BadRequest(new { error = result.ErrorMessage, code = "GENERATION_FAILED" });
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании документа");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ошибка при обработке запроса" });
            }
        }

        /// <summary>
        /// Получить список документов
        /// </summary>
        /// <param name="templateId">Фильтр по шаблону</param>
        /// <param name="customerId">Фильтр по заказчику</param>
        /// <param name="orderId">Фильтр по заказу</param>
        /// <param name="statusId">Фильтр по статусу</param>
        /// <returns>Список документов</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<Document>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(
            [FromQuery] Guid? templateId = null,
            [FromQuery] Guid? customerId = null,
            [FromQuery] Guid? orderId = null,
            [FromQuery] short? statusId = null)
        {
            try
            {
                var result = await _documentService.GetDocumentsAsync(templateId, customerId, orderId, statusId);

                if (!result.IsSuccess)
                {
                    return BadRequest(new { error = result.ErrorMessage });
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка документов");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ошибка при обработке запроса" });
            }
        }

        /// <summary>
        /// Получить документ по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор документа</param>
        /// <returns>Документ</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Document), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var result = await _documentService.GetDocumentByIdAsync(id);

                if (!result.IsSuccess)
                {
                    return NotFound(new { error = result.ErrorMessage });
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении документа");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ошибка при обработке запроса" });
            }
        }

        /// <summary>
        /// Скачать документ
        /// </summary>
        /// <param name="id">Идентификатор документа</param>
        /// <returns>Файл документа</returns>
        [HttpGet("{id}/download")]
        [ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Download(Guid id)
        {
            try
            {
                var result = await _documentService.DownloadDocumentAsync(id);
                if (!result.IsSuccess)
                {
                    return NotFound(new { error = result.ErrorMessage });
                }

                var downloadResult = result.Data;

                if (downloadResult == null)
                {
                    throw new Exception("Data is null");
                }

                var contentType = GetContentType(downloadResult.FileExtension);
                var fileName = $"{downloadResult.DocumentName}{downloadResult.FileExtension}";

                return File(downloadResult.FileStream, contentType, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при скачивании документа");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ошибка при обработке запроса" });
            }
        }

        /// <summary>
        /// Удалить документ
        /// </summary>
        /// <param name="id">Идентификатор документа</param>
        /// <returns>Результат операции</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var result = await _documentService.DeleteDocumentAsync(id);

                if (!result.IsSuccess)
                {
                    return NotFound(new { error = result.ErrorMessage });
                }

                return Ok(new { message = "Документ успешно удален" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении документа");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ошибка при обработке запроса" });
            }
        }

        /// <summary>
        /// Получить идентификатор текущего пользователя из claims
        /// </summary>
        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return string.IsNullOrEmpty(userIdClaim) ? Guid.Empty : Guid.Parse(userIdClaim);
        }

        /// <summary>
        /// Получить MIME-тип по расширению файла
        /// </summary>
        private string GetContentType(string fileExtension)
        {
            return fileExtension.ToLower() switch
            {
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".rtf" => "application/rtf",
                ".pdf" => "application/pdf",
                _ => "application/octet-stream"
            };
        }
    }
}
