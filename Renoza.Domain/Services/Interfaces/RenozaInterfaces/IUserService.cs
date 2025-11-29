using Renoza.Common.Helpers;
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
        /// <param name="name">Имя пользователя</param>
        /// <param name="displayName">Отображаемое имя</param>
        /// <param name="email">Email</param>
        /// <param name="phoneNumber">Номер телефона</param>
        /// <param name="phoneCountryCode">Код страны</param>
        /// <param name="password">Пароль</param>
        /// <returns>Результат регистрации с созданным пользователем</returns>
        Task<Result<User>> RegisterUserAsync(string name, string displayName, string email, string phoneNumber, string phoneCountryCode, string password);
    }
}
