using Renoza.Common.Base.Helpers;
using Renoza.Domain.Entities.Auth;
using Renoza.Domain.Entities.Users;
using Renoza.Domain.Services.Interfaces.BaseInterfaces;
using Renoza.Infrastructure.Contexts;

namespace Renoza.Domain.Services.Interfaces.RenozaInterfaces
{
    /// <summary>
    /// Интерфейс сервиса работы с пользователями
    /// </summary>
    public interface IUserService : IBaseService<RenozaContext>
    {
        /// <summary>
        /// Возвращает интерфейс для запроса пользователей
        /// </summary>
        /// <returns>Интерфейс для запроса пользователей</returns>
        IQueryable<User> GetQueryable();

        /// <summary>
        /// Зарегистрировать нового пользователя
        /// </summary>
        /// <param name="input">Входные данные для регистрации</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат регистрации с созданным пользователем</returns>
        Task<Result<User>> RegisterUserAsync(RegisterUserInput input, CancellationToken cancellationToken = default);
    }
}
