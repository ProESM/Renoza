using Renoza.Domain.Entities.Profiles;
using Renoza.Domain.Services.Interfaces.BaseInterfaces;
using Renoza.Infrastructure.Contexts;

namespace Renoza.Domain.Services.Interfaces.RenozaInterfaces
{
    /// <summary>
    /// Интерфейс сервиса работы с профилями заказчиков
    /// </summary>
    public interface ICustomerProfileService : IBaseService<RenozaContext>
    {
        /// <summary>
        /// Возвращает интерфейс для запроса профилей заказчиков
        /// </summary>
        /// <returns>Интерфейс для запроса профилей заказчиков</returns>
        IQueryable<CustomerProfile> GetQueryable();

        /// <summary>
        /// Получить профиль заказчика по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор профиля</param>
        /// <returns>Профиль заказчика</returns>
        Task<CustomerProfile?> GetByIdAsync(Guid id);

        /// <summary>
        /// Получить профиль заказчика по идентификатору пользователя
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns>Профиль заказчика</returns>
        Task<CustomerProfile?> GetByUserIdAsync(Guid userId);

        /// <summary>
        /// Создать профиль заказчика
        /// </summary>
        /// <param name="profile">Профиль заказчика</param>
        /// <returns>Созданный профиль</returns>
        Task<CustomerProfile> CreateAsync(CustomerProfile profile);

        /// <summary>
        /// Обновить профиль заказчика
        /// </summary>
        /// <param name="profile">Профиль заказчика</param>
        /// <returns>Обновленный профиль</returns>
        Task<CustomerProfile> UpdateAsync(CustomerProfile profile);

        /// <summary>
        /// Деактивировать профиль заказчика
        /// </summary>
        /// <param name="id">Идентификатор профиля</param>
        /// <returns>Успешность операции</returns>
        Task<bool> DeactivateAsync(Guid id);
    }
}
