using Renoza.Domain.Entities.Profiles;
using Renoza.Domain.Services.Interfaces.BaseInterfaces;
using Renoza.Infrastructure.Contexts;

namespace Renoza.Domain.Services.Interfaces.RenozaInterfaces
{
    /// <summary>
    /// Интерфейс сервиса работы с профилями технического надзора
    /// </summary>
    public interface ITechnicalSupervisorProfileService : IBaseService<RenozaContext>
    {
        /// <summary>
        /// Возвращает интерфейс для запроса профилей технического надзора
        /// </summary>
        /// <returns>Интерфейс для запроса профилей</returns>
        IQueryable<TechnicalSupervisorProfile> GetQueryable();

        /// <summary>
        /// Получить профиль по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор профиля</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Профиль технического надзора</returns>
        Task<TechnicalSupervisorProfile?> GetByIdAsync(Guid id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить профиль по идентификатору пользователя
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Профиль технического надзора</returns>
        Task<TechnicalSupervisorProfile?> GetByUserIdAsync(Guid userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Создать профиль технического надзора
        /// </summary>
        /// <param name="profile">Профиль технического надзора</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Созданный профиль</returns>
        Task<TechnicalSupervisorProfile> CreateAsync(TechnicalSupervisorProfile profile,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Создать профиль технического надзора с созданием профиля компании и задания на верификацию
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <param name="companyInn">ИНН компании</param>
        /// <param name="ipAddress">IP адрес для создания задания на верификацию</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Созданный профиль</returns>
        Task<TechnicalSupervisorProfile> CreateWithCompanyAsync(Guid userId, string companyInn, string ipAddress,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Обновить профиль технического надзора
        /// </summary>
        /// <param name="profile">Профиль технического надзора</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Обновленный профиль</returns>
        Task<TechnicalSupervisorProfile> UpdateAsync(TechnicalSupervisorProfile profile,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Деактивировать профиль технического надзора
        /// </summary>
        /// <param name="id">Идентификатор профиля</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Успешность операции</returns>
        Task<bool> DeactivateAsync(Guid id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Установить доступность для новых заказов
        /// </summary>
        /// <param name="id">Идентификатор профиля</param>
        /// <param name="isAvailable">Доступность</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Успешность операции</returns>
        Task<bool> SetAvailabilityAsync(Guid id, bool isAvailable,
            CancellationToken cancellationToken = default);
    }
}
