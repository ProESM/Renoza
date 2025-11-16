using Renoza.Domain.Entities.Profiles;
using Renoza.Domain.Services.Interfaces.BaseInterfaces;
using Renoza.Infrastructure.Contexts;

namespace Renoza.Domain.Services.Interfaces.RenozaInterfaces
{
    /// <summary>
    /// Интерфейс сервиса работы с профилями работников
    /// </summary>
    public interface IWorkerProfileService : IBaseService<RenozaContext>
    {
        /// <summary>
        /// Возвращает интерфейс для запроса профилей работников
        /// </summary>
        /// <returns>Интерфейс для запроса профилей работников</returns>
        IQueryable<WorkerProfile> GetQueryable();

        /// <summary>
        /// Получить профиль работника по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор профиля</param>
        /// <returns>Профиль работника</returns>
        Task<WorkerProfile?> GetByIdAsync(Guid id);

        /// <summary>
        /// Получить профиль работника по идентификатору пользователя
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns>Профиль работника</returns>
        Task<WorkerProfile?> GetByUserIdAsync(Guid userId);

        /// <summary>
        /// Создать профиль работника
        /// </summary>
        /// <param name="profile">Профиль работника</param>
        /// <returns>Созданный профиль</returns>
        Task<WorkerProfile> CreateAsync(WorkerProfile profile);

        /// <summary>
        /// Обновить профиль работника
        /// </summary>
        /// <param name="profile">Профиль работника</param>
        /// <returns>Обновленный профиль</returns>
        Task<WorkerProfile> UpdateAsync(WorkerProfile profile);

        /// <summary>
        /// Деактивировать профиль работника
        /// </summary>
        /// <param name="id">Идентификатор профиля</param>
        /// <returns>Успешность операции</returns>
        Task<bool> DeactivateAsync(Guid id);

        /// <summary>
        /// Установить доступность работника
        /// </summary>
        /// <param name="id">Идентификатор профиля</param>
        /// <param name="isAvailable">Доступность</param>
        /// <returns>Успешность операции</returns>
        Task<bool> SetAvailabilityAsync(Guid id, bool isAvailable);
    }
}
