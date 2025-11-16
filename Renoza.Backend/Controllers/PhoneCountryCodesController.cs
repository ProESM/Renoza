using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;

namespace Renoza.Backend.Controllers
{
    /// <summary>
    /// Контроллер для работы с международными телефонными кодами
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PhoneCountryCodesController : ControllerBase
    {
        private readonly ILogger<PhoneCountryCodesController> _logger;
        private readonly IPhoneCountryCodeService _phoneCountryCodeService;

        public PhoneCountryCodesController(
            ILogger<PhoneCountryCodesController> logger,
            IPhoneCountryCodeService phoneCountryCodeService)
        {
            _logger = logger;
            _phoneCountryCodeService = phoneCountryCodeService;
        }

        /// <summary>
        /// Получить список активных международных телефонных кодов
        /// </summary>
        /// <returns>Список международных телефонных кодов с информацией о странах</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> GetPhoneCountryCodes()
        {
            try
            {
                var phoneCountryCodes = await _phoneCountryCodeService.GetActiveAsync();

                var response = phoneCountryCodes.Select(x => new
                {
                    id = x.Id,
                    code = x.Code,
                    phoneFormat = x.PhoneFormat,
                    countryId = x.CountryId,
                    countryCode = x.Country.Code,
                    countryName = x.Country.Name
                }).ToList();

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка международных телефонных кодов");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Произошла ошибка при обработке запроса" });
            }
        }
    }
}
