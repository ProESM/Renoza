using Renoza.Domain.Entities.Users;
using Renoza.Domain.Services.Interfaces.BaseInterfaces;

namespace Renoza.Domain.Services.Interfaces.RenozaInterfaces
{
    /// <summary>
    /// Интерфейс сервиса работы с пользователями
    /// </summary>
    public interface IUserService : IBaseService
    {
        /// <summary>
        /// Возвращает интерфейс для запроса пользователей
        /// </summary>
        /// <returns>Интерфейс для запроса пользователей</returns>
        IQueryable<User> GetQueryable();
    }
}
