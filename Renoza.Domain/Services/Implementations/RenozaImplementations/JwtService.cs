using Microsoft.IdentityModel.Tokens;
using Renoza.Domain.Entities.Users;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Renoza.Domain.Options;

namespace Renoza.Domain.Services.Implementations.RenozaImplementations
{
    /// <summary>
    /// Сервис для работы с JWT токенами
    /// </summary>
    public class JwtService : IJwtService
    {
        #region Настройки

        /// <summary>
        /// Настройки JWT
        /// </summary>
        private readonly JwtSettings _jwtSettings;

        #endregion

        /// <summary>
        /// Сервис для работы с JWT токенами
        /// </summary>
        /// <param name="jwtSettings">Настройки JWT</param>
        public JwtService(JwtSettings jwtSettings)
        {
            _jwtSettings = jwtSettings;
        }

        /// <summary>
        /// Генерирует JWT токен для пользователя с указанной ролью
        /// </summary>
        /// <param name="user">Пользователь, для которого генерируется токен</param>
        /// <param name="roleId">Идентификатор роли пользователя</param>
        /// <returns>Кортеж, содержащий сгенерированный JWT токен и дату его истечения</returns>
        public (string Token, DateTime ExpiresAt) GenerateToken(User user, Guid roleId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);
            var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes ?? 60);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, roleId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiresAt,
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return (tokenString, expiresAt);
        }

        /// <summary>
        /// Валидирует JWT токен и извлекает из него данные пользователя
        /// </summary>
        /// <param name="token">JWT токен для валидации</param>
        /// <returns>ClaimsPrincipal с данными пользователя, если токен валиден; null, если токен невалиден или истек</returns>
        public ClaimsPrincipal? ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                return principal;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Извлекает идентификатор пользователя из JWT токена
        /// </summary>
        /// <param name="token">JWT токен</param>
        /// <returns>Идентификатор пользователя, если токен валиден и содержит корректный ID; null в противном случае</returns>
        public int? GetUserIdFromToken(string token)
        {
            var principal = ValidateToken(token);
            if (principal == null)
                return null;

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return null;

            return userId;
        }
    }
}
