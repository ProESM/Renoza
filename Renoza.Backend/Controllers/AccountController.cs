using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Renoza.Backend.Models.Password;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;
using Renoza.Domain.Validators;
using System.Security.Claims;

namespace Renoza.Backend.Controllers
{
    /// <summary>
    /// Контроллер аккаунта
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IPasswordService _passwordService;
        private readonly PasswordValidator _passwordValidator;

        public AccountController(IPasswordService passwordService, PasswordValidator passwordValidator)
        {
            _passwordService = passwordService;
            _passwordValidator = passwordValidator;
        }

        [HttpPost("change-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ChangePassword([FromBody] PasswordChangeRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized(new { message = "Пользователь не авторизован" });
            }

            var userId = Guid.Parse(userIdClaim);

            // Валидация нового пароля
            var (isValid, errorMessage) = _passwordValidator.Validate(request.NewPassword);
            if (!isValid)
            {
                return BadRequest(new { message = errorMessage });
            }

            // Проверка подтверждения пароля
            if (request.NewPassword != request.ConfirmPassword)
            {
                return BadRequest(new { message = "Пароли не совпадают" });
            }

            // Смена пароля
            var result = await _passwordService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword);

            if (!result)
            {
                return BadRequest(new { message = "Не удалось сменить пароль. Проверьте текущий пароль или убедитесь, что новый пароль не использовался в последнее время." });
            }

            return Ok(new { message = "Пароль успешно изменён" });
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(Guid userId, string newPassword)
        {
            // Валидация пароля
            var (isValid, errorMessage) = _passwordValidator.Validate(newPassword);
            if (!isValid)
            {
                return BadRequest(new { message = errorMessage });
            }

            var result = await _passwordService.SetPasswordAsync(userId, newPassword);

            if (!result)
            {
                return BadRequest(new { message = "Сброс пароля не удался. Возможно, этот пароль был недавно использован." });
            }

            return Ok(new { message = "Пароль успешно сброшен" });
        }
    }
}
