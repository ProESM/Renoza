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
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Профиль работника</returns>
        Task<WorkerProfile?> GetByIdAsync(Guid id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить профиль работника по идентификатору пользователя
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Профиль работника</returns>
        Task<WorkerProfile?> GetByUserIdAsync(Guid userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Создать профиль работника
        /// </summary>
        /// <param name="profile">Профиль работника</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Созданный профиль</returns>
        Task<WorkerProfile> CreateAsync(WorkerProfile profile,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Создать профиль работника с созданием профиля компании и задания на верификацию
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <param name="companyInn">ИНН компании</param>
        /// <param name="ipAddress">IP адрес для создания задания на верификацию</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Созданный профиль работника</returns>
        Task<WorkerProfile> CreateWithCompanyAsync(Guid userId, string companyInn, string ipAddress,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Обновить профиль работника
        /// </summary>
        /// <param name="profile">Профиль работника</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Обновленный профиль</returns>
        Task<WorkerProfile> UpdateAsync(WorkerProfile profile,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Деактивировать профиль работника
        /// </summary>
        /// <param name="id">Идентификатор профиля</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Успешность операции</returns>
        Task<bool> DeactivateAsync(Guid id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Установить доступность работника
        /// </summary>
        /// <param name="id">Идентификатор профиля</param>
        /// <param name="isAvailable">Доступность</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Успешность операции</returns>
        Task<bool> SetAvailabilityAsync(Guid id, bool isAvailable,
            CancellationToken cancellationToken = default);
    }
}
