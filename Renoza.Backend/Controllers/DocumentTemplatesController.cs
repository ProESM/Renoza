using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Renoza.Backend.Attributes;
using Renoza.Domain.Entities.Documents;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;
using System.Security.Claims;

namespace Renoza.Backend.Controllers
{
    /// <summary>
    /// Контроллер для работы с шаблонами документов
    /// </summary>
    /// <remarks>
    /// Требуется верификация email ИЛИ телефона для доступа к методам контроллера.
    /// Примеры использования атрибута RequireVerification:
    /// - [RequireVerification] - требуется email ИЛИ телефон (по умолчанию)
    /// - [RequireVerification(RequireBoth = true)] - требуется И email И телефон
    /// - [RequireVerification(RequireEmailVerification = true, RequirePhoneVerification = false)] - только email
    /// - [RequireVerification(RequireEmailVerification = false, RequirePhoneVerification = true)] - только телефон
    /// </remarks>
    [ApiController]
    [Route("api/document-templates")]
    [Authorize]
    [RequireVerification]
    public class DocumentTemplatesController : ControllerBase
    {
        private readonly IDocumentTemplateService _templateService;
        private readonly ILogger<DocumentTemplatesController> _logger;

        public DocumentTemplatesController(
            IDocumentTemplateService templateService,
            ILogger<DocumentTemplatesController> logger)
        {
            _templateService = templateService;
            _logger = logger;
        }

        // ВРЕМЕННО ЗАКОММЕНТИРОВАНО ИЗ-ЗА ПРОБЛЕМ СО SWAGGER
        // TODO: Исправить конфигурацию Swagger для работы с IFormFile
        /*
        /// <summary>
        /// Создать шаблон документа
        /// </summary>
        /// <param name="file">Файл шаблона</param>
        /// <param name="templateTypeId">Тип шаблона</param>
        /// <param name="name">Наименование шаблона</param>
        /// <param name="description">Описание шаблона</param>
        /// <returns>Идентификатор созданного шаблона</returns>
        [HttpPost]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(
            [FromForm] IFormFile file,
            [FromForm] short templateTypeId,
            [FromForm] string name,
            [FromForm] string? description)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { error = "Файл шаблона обязателен", code = "VALIDATION_ERROR" });
                }

                var userId = GetCurrentUserId();
                if (userId == Guid.Empty)
                {
                    return Unauthorized(new { message = "Пользователь не авторизован" });
                }

                // Читаем содержимое файла
                byte[] fileContent;
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    fileContent = memoryStream.ToArray();
                }

                var input = new UploadTemplateInput
                {
                    TemplateTypeId = templateTypeId,
                    Name = name,
                    Description = description,
                    CreatedBy = userId,
                    FileContent = fileContent,
                    FileName = file.FileName
                };

                var result = await _templateService.UploadTemplateAsync(input);

                if (!result.IsSuccess)
                {
                    return BadRequest(new { error = result.ErrorMessage, code = "UPLOAD_FAILED" });
                }

                return Ok(new { templateId = result.Data, message = "Шаблон успешно создан" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании шаблона");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ошибка при обработке запроса" });
            }
        }
        */

        /// <summary>
        /// Получить список шаблонов
        /// </summary>
        /// <param name="templateTypeId">Фильтр по типу шаблона</param>
        /// <param name="isActive">Фильтр по активности</param>
        /// <returns>Список шаблонов</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<DocumentTemplate>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(
            [FromQuery] short? templateTypeId = null,
            [FromQuery] bool? isActive = null)
        {
            try
            {
                var result = await _templateService.GetTemplatesAsync(templateTypeId, isActive);

                if (!result.IsSuccess)
                {
                    return BadRequest(new { error = result.ErrorMessage });
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка шаблонов");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ошибка при обработке запроса" });
            }
        }

        /// <summary>
        /// Получить шаблон по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор шаблона</param>
        /// <returns>Шаблон</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DocumentTemplate), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var result = await _templateService.GetTemplateByIdAsync(id);

                if (!result.IsSuccess)
                {
                    return NotFound(new { error = result.ErrorMessage });
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении шаблона");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ошибка при обработке запроса" });
            }
        }

        /// <summary>
        /// Удалить шаблон
        /// </summary>
        /// <param name="id">Идентификатор шаблона</param>
        /// <returns>Результат операции</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var result = await _templateService.DeleteTemplateAsync(id);

                if (!result.IsSuccess)
                {
                    return NotFound(new { error = result.ErrorMessage });
                }

                return Ok(new { message = "Шаблон успешно удален" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении шаблона");
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
    }
}
