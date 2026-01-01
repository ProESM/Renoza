using Renoza.Domain.Entities.Roles;
using Renoza.Domain.Services.Interfaces.BaseInterfaces;
using Renoza.Infrastructure.Contexts;

namespace Renoza.Domain.Services.Interfaces.RenozaInterfaces
{
    /// <summary>
    /// Интерфейс сервиса работы с ролями
    /// </summary>
    public interface IRoleService : IBaseService<RenozaContext>
    {
        /// <summary>
        /// Возвращает интерфейс для запроса ролей
        /// </summary>
        /// <returns>Интерфейс для запроса ролей</returns>
        IQueryable<Role> GetQueryable();

        /// <summary>
        /// Получить роль по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор роли</param>
        /// <returns>Роль</returns>
        Task<Role?> GetByIdAsync(Guid id);

        /// <summary>
        /// Получить идентификатор роли по имени
        /// </summary>
        /// <param name="roleName">Имя роли (Customer, Worker, TechnicalSupervisor)</param>
        /// <returns>Идентификатор роли или null</returns>
        Guid? GetRoleIdByName(string roleName);

        /// <summary>
        /// Получить роли пользователя
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns>Список ролей пользователя</returns>
        Task<List<Role>> GetUserRolesAsync(Guid userId);

        /// <summary>
        /// Назначить роль пользователю
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="roleId">Идентификатор роли</param>
        /// <returns>Успешность операции</returns>
        Task<bool> AssignRoleToUserAsync(Guid userId, Guid roleId);

        /// <summary>
        /// Отозвать роль у пользователя
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="roleId">Идентификатор роли</param>
        /// <returns>Успешность операции</returns>
        Task<bool> RevokeRoleFromUserAsync(Guid userId, Guid roleId);
    }
}
