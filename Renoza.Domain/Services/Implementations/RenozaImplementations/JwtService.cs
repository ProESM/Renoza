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
        private readonly JwtSettings _jwtSettings;

        public JwtService(JwtSettings jwtSettings)
        {
            _jwtSettings = jwtSettings;
        }

        /// <summary>
        /// Генерация JWT токена для пользователя
        /// </summary>
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
        /// Валидация JWT токена
        /// </summary>
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
        /// Получение ID пользователя из токена
        /// </summary>
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
