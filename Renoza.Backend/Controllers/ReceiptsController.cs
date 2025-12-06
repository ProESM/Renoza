using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Renoza.Backend.Models.Receipts;
using Renoza.Domain.Exceptions;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;

namespace Renoza.Backend.Controllers
{
    /// <summary>
    /// Контроллер для работы с чеками (кассовые чеки, товарные накладные и т.д.)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReceiptsController : ControllerBase
    {
        private readonly IReceiptService _receiptService;
        private readonly ILogger<ReceiptsController> _logger;

        public ReceiptsController(IReceiptService receiptService, ILogger<ReceiptsController> logger)
        {
            _receiptService = receiptService;
            _logger = logger;
        }

        /// <summary>
        /// Загрузить кассовый чек
        /// </summary>
        /// <param name="request">Данные кассового чека</param>
        /// <returns>Информация о принятии чека в обработку</returns>
        [HttpPost("cash-receipt")]
        [ProducesResponseType(typeof(UploadReceiptResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> UploadCashReceipt([FromBody] UploadCashReceiptRequest request)
        {
            try
            {
                // Получаем IP адрес клиента
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                // Загружаем чек через сервис
                var jobId = await _receiptService.UploadCashReceiptAsync(
                    ipAddress,
                    request.InputType,
                    request.Data,
                    request.ContentType,
                    request.FileName);

                return Ok(new UploadReceiptResponse
                {
                    JobId = jobId,
                    Message = "Кассовый чек принят в обработку",
                    UploadedAt = DateTime.UtcNow
                });
            }
            catch (RateLimitExceededException rateLimitExceededException)
            {
                _logger.LogError(rateLimitExceededException, "Ошибка при загрузке кассового чека");
                return StatusCode(StatusCodes.Status429TooManyRequests, new
                {
                    message = "Ошибка при обработке запроса"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при загрузке кассового чека");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "Ошибка при обработке запроса"
                });
            }
        }
    }
}
