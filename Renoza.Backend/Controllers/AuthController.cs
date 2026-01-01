using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Renoza.Backend.Models.Auth;
using Renoza.Domain.Entities.Auth;
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
        private readonly IEmailVerificationService _emailVerificationService;
        private readonly IPhoneVerificationService _phoneVerificationService;
        private readonly IRoleService _roleService;

        public AuthController(
            ILogger<AuthController> logger,
            IJwtService jwtService,
            IUserService userService,
            IPasswordService passwordService,
            IEmailVerificationService emailVerificationService,
            IPhoneVerificationService phoneVerificationService,
            IRoleService roleService)
        {
            _logger = logger;
            _jwtService = jwtService;
            _userService = userService;
            _passwordService = passwordService;
            _emailVerificationService = emailVerificationService;
            _phoneVerificationService = phoneVerificationService;
            _roleService = roleService;
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
                        .AsNoTracking()
                        .FirstOrDefaultAsync(u => (u.PhoneCountryCode + u.PhoneNumber) == username);
                }
                else
                {
                    user = await _userService.GetQueryable()
                        .AsNoTracking()
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

                // Получаем роли пользователя
                var userRoles = await _roleService.GetUserRolesAsync(user.Id);
                if (userRoles == null || userRoles.Count == 0)
                {
                    _logger.LogWarning("Пользователь {Username} не имеет назначенных ролей", request.Username);
                    return Unauthorized(new { message = "Пользователь не имеет назначенных ролей" });
                }

                // Определяем выбранную роль
                Guid selectedRoleId;
                if (request.SelectedRoleId.HasValue)
                {
                    // Проверяем, что выбранная роль есть у пользователя
                    var selectedRole = userRoles.FirstOrDefault(r => r.Id == request.SelectedRoleId.Value);
                    if (selectedRole == null)
                    {
                        _logger.LogWarning("Пользователь {Username} попытался использовать роль {RoleId}, которая ему не назначена",
                            request.Username, request.SelectedRoleId.Value);
                        return Unauthorized(new { message = "Выбранная роль не назначена пользователю" });
                    }
                    selectedRoleId = request.SelectedRoleId.Value;
                }
                else
                {
                    // Берем первую активную роль
                    selectedRoleId = userRoles.First().Id;
                }

                // Генерация JWT токена
                var (token, expiresAt) = _jwtService.GenerateToken(user, selectedRoleId);

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

                // Получаем текущую активную роль из JWT
                var currentRoleIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Role);
                Guid? currentRoleId = null;
                if (currentRoleIdClaim != null && Guid.TryParse(currentRoleIdClaim.Value, out var roleId))
                {
                    currentRoleId = roleId;
                }

                var user = await _userService.GetQueryable()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    return NotFound(new { message = "Пользователь не найден" });
                }

                // Получаем все роли пользователя
                var userRoles = await _roleService.GetUserRolesAsync(userId);

                return Ok(new
                {
                    id = user.Id,
                    name = user.Name,
                    isActive = user.IsActive,
                    currentRoleId = currentRoleId,
                    roles = userRoles.Select(r => new
                    {
                        id = r.Id,
                        name = r.Name,
                        description = r.Description
                    })
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении информации о текущем пользователе");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Произошла ошибка при обработке запроса" });
            }
        }

        /// <summary>
        /// Регистрация нового пользователя
        /// </summary>
        /// <param name="request">Данные для регистрации</param>
        /// <returns>Информация о зарегистрированном пользователе</returns>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<RegisterResponse>> Register([FromBody] RegisterRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Получаем IP адрес клиента
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                // Регистрация пользователя
                var input = new RegisterUserInput
                {
                    Name = request.Name,
                    DisplayName = request.DisplayName,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    PhoneCountryCode = request.PhoneCountryCode,
                    Password = request.Password,
                    UserRole = request.UserRole,
                    CompanyInn = request.CompanyInn,
                    IpAddress = ipAddress
                };

                var result = await _userService.RegisterUserAsync(input);

                if (!result.IsSuccess)
                {
                    return BadRequest(new { message = result.ErrorMessage });
                }

                var user = result.Data;

                // Отправляем коды верификации
                var emailVerificationResult = await _emailVerificationService.CreateVerificationCodeAsync(user!.Id, user.Email);
                var phoneVerificationResult = await _phoneVerificationService.CreateVerificationCodeAsync(
                    user.Id,
                    user.PhoneNumber,
                    user.PhoneCountryCode);

                if (!emailVerificationResult.IsSuccess)
                {
                    _logger.LogWarning("Не удалось создать код верификации email для пользователя {UserId}: {Error}",
                        user.Id, emailVerificationResult.ErrorMessage);
                }

                if (!phoneVerificationResult.IsSuccess)
                {
                    _logger.LogWarning("Не удалось создать код верификации телефона для пользователя {UserId}: {Error}",
                        user.Id, phoneVerificationResult.ErrorMessage);
                }

                _logger.LogInformation("Успешная регистрация пользователя: {Username}", user.Name);

                var response = new RegisterResponse
                {
                    UserId = user.Id,
                    Username = user.Name,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    PhoneCountryCode = user.PhoneCountryCode
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при регистрации пользователя: {Username}", request.Name);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Произошла ошибка при обработке запроса" });
            }
        }

        /// <summary>
        /// Верификация email
        /// </summary>
        /// <param name="request">Данные для верификации</param>
        /// <returns>Результат верификации</returns>
        [HttpPost("verify-email")]
        [AllowAnonymous]
        public async Task<ActionResult> VerifyEmail([FromBody] VerifyEmailRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _emailVerificationService.VerifyEmailAsync(request.Email, request.Code);

                if (!result.IsSuccess)
                {
                    return BadRequest(new { message = result.ErrorMessage });
                }

                _logger.LogInformation("Email успешно верифицирован: {Email}", request.Email);

                return Ok(new { message = "Email успешно подтвержден" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при верификации email: {Email}", request.Email);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Произошла ошибка при обработке запроса" });
            }
        }

        /// <summary>
        /// Верификация телефона
        /// </summary>
        /// <param name="request">Данные для верификации</param>
        /// <returns>Результат верификации</returns>
        [HttpPost("verify-phone")]
        [AllowAnonymous]
        public async Task<ActionResult> VerifyPhone([FromBody] VerifyPhoneRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _phoneVerificationService.VerifyPhoneAsync(
                    request.PhoneNumber,
                    request.PhoneCountryCode,
                    request.Code);

                if (!result.IsSuccess)
                {
                    return BadRequest(new { message = result.ErrorMessage });
                }

                _logger.LogInformation("Телефон успешно верифицирован: {PhoneCountryCode}{PhoneNumber}",
                    request.PhoneCountryCode, request.PhoneNumber);

                return Ok(new { message = "Телефон успешно подтвержден" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при верификации телефона: {PhoneCountryCode}{PhoneNumber}",
                    request.PhoneCountryCode, request.PhoneNumber);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Произошла ошибка при обработке запроса" });
            }
        }

        /// <summary>
        /// Отправка кода верификации
        /// </summary>
        /// <param name="request">Данные для отправки кода</param>
        /// <returns>Результат отправки</returns>
        [HttpPost("send-verification-code")]
        [AllowAnonymous]
        public async Task<ActionResult> SendVerificationCode([FromBody] SendVerificationCodeRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var verificationType = request.VerificationType.ToLower();

                if (verificationType == "email")
                {
                    if (string.IsNullOrWhiteSpace(request.Email))
                    {
                        return BadRequest(new { message = "Email обязателен для верификации email" });
                    }

                    var result = await _emailVerificationService.ResendVerificationCodeAsync(request.UserId, request.Email);

                    if (!result.IsSuccess)
                    {
                        return BadRequest(new { message = result.ErrorMessage });
                    }

                    _logger.LogInformation("Код верификации email отправлен пользователю {UserId}", request.UserId);

                    return Ok(new { message = "Код верификации отправлен на email" });
                }
                else if (verificationType == "phone")
                {
                    if (string.IsNullOrWhiteSpace(request.PhoneNumber) || string.IsNullOrWhiteSpace(request.PhoneCountryCode))
                    {
                        return BadRequest(new { message = "Номер телефона и код страны обязательны для верификации телефона" });
                    }

                    var result = await _phoneVerificationService.ResendVerificationCodeAsync(
                        request.UserId,
                        request.PhoneNumber,
                        request.PhoneCountryCode);

                    if (!result.IsSuccess)
                    {
                        return BadRequest(new { message = result.ErrorMessage });
                    }

                    _logger.LogInformation("Код верификации телефона отправлен пользователю {UserId}", request.UserId);

                    return Ok(new { message = "Код верификации отправлен на телефон" });
                }
                else
                {
                    return BadRequest(new { message = "Неверный тип верификации. Используйте 'email' или 'phone'" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при отправке кода верификации для пользователя {UserId}", request.UserId);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Произошла ошибка при обработке запроса" });
            }
        }
    }
}
