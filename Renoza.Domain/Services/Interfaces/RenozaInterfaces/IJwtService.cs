using Renoza.Domain.Entities.Users;
using System.Security.Claims;

namespace Renoza.Domain.Services.Interfaces.RenozaInterfaces
{
    /// <summary>
    /// Интерфейс сервиса для работы с JWT токенами
    /// </summary>
    public interface IJwtService
    {
        /// <summary>
        /// Генерация JWT токена для пользователя
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <param name="roleId">Идентификатор роли для текущей сессии</param>
        /// <returns>JWT токен и время истечения</returns>
        (string Token, DateTime ExpiresAt) GenerateToken(User user, Guid roleId);

        /// <summary>
        /// Валидация JWT токена
        /// </summary>
        /// <param name="token">JWT токен</param>
        /// <returns>ClaimsPrincipal если токен валиден, иначе null</returns>
        ClaimsPrincipal? ValidateToken(string token);

        /// <summary>
        /// Получение ID пользователя из токена
        /// </summary>
        /// <param name="token">JWT токен</param>
        /// <returns>ID пользователя или null</returns>
        int? GetUserIdFromToken(string token);
    }
}
