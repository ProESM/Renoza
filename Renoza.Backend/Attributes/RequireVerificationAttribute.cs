using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;
using System.Security.Claims;

namespace Renoza.Backend.Attributes
{
    /// <summary>
    /// Атрибут для проверки верификации пользователя (email или телефон)
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequireVerificationAttribute : Attribute, IAsyncAuthorizationFilter
    {
        /// <summary>
        /// Требуется ли верификация email
        /// </summary>
        public bool RequireEmailVerification { get; set; } = true;

        /// <summary>
        /// Требуется ли верификация телефона
        /// </summary>
        public bool RequirePhoneVerification { get; set; } = true;

        /// <summary>
        /// Логика проверки: true - требуется И email И телефон, false - требуется ИЛИ email ИЛИ телефон
        /// </summary>
        public bool RequireBoth { get; set; } = false;

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // Получаем UserId из claims
            var userIdClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                context.Result = new UnauthorizedObjectResult(new { message = "Невалидный токен" });
                return;
            }

            // Получаем IUserService из DI
            var userService = context.HttpContext.RequestServices.GetService<IUserService>();
            if (userService == null)
            {
                context.Result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
                return;
            }

            // Получаем пользователя
            var user = await userService.GetQueryable()
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                context.Result = new NotFoundObjectResult(new { message = "Пользователь не найден" });
                return;
            }

            // Проверяем верификацию
            bool isEmailVerified = !RequireEmailVerification || user.IsEmailVerified;
            bool isPhoneVerified = !RequirePhoneVerification || user.IsPhoneNumberVerified;

            bool isVerified = RequireBoth
                ? (isEmailVerified && isPhoneVerified)
                : (isEmailVerified || isPhoneVerified);

            if (!isVerified)
            {
                var missingVerifications = new List<string>();
                if (RequireEmailVerification && !user.IsEmailVerified)
                    missingVerifications.Add("email");
                if (RequirePhoneVerification && !user.IsPhoneNumberVerified)
                    missingVerifications.Add("телефон");

                var message = RequireBoth
                    ? $"Требуется верификация: {string.Join(" и ", missingVerifications)}"
                    : $"Требуется верификация хотя бы одного из: {string.Join(" или ", missingVerifications)}";

                context.Result = new ObjectResult(new { message })
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }
        }
    }
}
