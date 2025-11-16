using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Renoza.Backend.Models.Auth;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;
using System.Text.RegularExpressions;

namespace Renoza.Backend.Controllers
{
    /// <summary>
    /// Контроллер авторизации
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IJwtService _jwtService;
        private readonly IUserService _userService;
        private readonly IPasswordService _passwordService;

        public AuthController(
            ILogger<AuthController> logger,
            IJwtService jwtService,
            IUserService userService,
            IPasswordService passwordService)
        {
            _logger = logger;
            _jwtService = jwtService;
            _userService = userService;
            _passwordService = passwordService;
        }

        /// <summary>
        /// Авторизация пользователя
        /// </summary>
        /// <param name="request">Данные для входа</param>
        /// <returns>JWT токен и информация о пользователе</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Поиск пользователя в зависимости от типа логина
                var username = request.Username;
                var loginType = request.LoginType?.ToLower();

                if (!string.IsNullOrWhiteSpace(username))
                {
                    username = username.Trim();
                }

                Domain.Entities.Users.User? user = null;

                if (loginType == "phone")
                {
                    if (!string.IsNullOrWhiteSpace(username))
                    {
                        username = Regex.Replace(username, @"\D", "");
                    }
                    // Поиск по номеру телефона (формат: +7XXXXXXXXXX)
                    user = await _userService.GetQueryable()
                        .FirstOrDefaultAsync(u => (u.PhoneCountryCode + u.PhoneNumber) == username);
                }
                else
                {
                    user = await _userService.GetQueryable()
                        .FirstOrDefaultAsync(u => u.Email == username || u.Name == username);
                }

                if (user == null)
                {
                    _logger.LogWarning("Попытка входа с несуществующим пользователем: {Username}", request.Username);
                    return Unauthorized(new { message = "Неверное имя пользователя или пароль" });
                }

                // TODO: В продакшене здесь должна быть проверка хешированного пароля
                // Например: if (!PasswordHasher.Verify(request.Password, user.PasswordHash))
                // Сейчас для демонстрации просто проверяем, что пароль не пустой
                //if (string.IsNullOrEmpty(request.Password))
                //{
                //    _logger.LogWarning("Попытка входа с пустым паролем для пользователя: {Username}", request.Username);
                //    return Unauthorized(new { message = "Неверное имя пользователя или пароль" });
                //}

                if (!await _passwordService.ValidatePasswordAsync(user.Id, request.Password))
                {
                    _logger.LogWarning("Попытка входа с неверным паролем для пользователя: {Username}", request.Username);
                    return Unauthorized(new { message = "Неверное имя пользователя или пароль" });
                }

                // Генерация JWT токена
                var (token, expiresAt) = _jwtService.GenerateToken(user);

                var response = new AuthResponse
                {
                    Token = token,
                    ExpiresAt = expiresAt,
                    TokenType = "Bearer",
                    UserId = user.Id,
                    Username = user.Name
                };

                _logger.LogInformation("Успешная авторизация пользователя: {Username}", user.Name);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при авторизации пользователя: {Username}", request.Username);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Произошла ошибка при обработке запроса" });
            }
        }

        /// <summary>
        /// Получение информации о текущем пользователе
        /// </summary>
        /// <returns>Информация о текущем пользователе</returns>
        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult> GetCurrentUser()
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                {
                    return Unauthorized(new { message = "Невалидный токен" });
                }

                var user = await _userService.GetQueryable()
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    return NotFound(new { message = "Пользователь не найден" });
                }

                return Ok(new
                {
                    id = user.Id,
                    name = user.Name,
                    isActive = user.IsActive
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении информации о текущем пользователе");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Произошла ошибка при обработке запроса" });
            }
        }
    }
}
