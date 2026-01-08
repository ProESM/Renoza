using Renoza.Common.Base.Helpers;
using Renoza.Domain.Entities.CompanyMembers;

namespace Renoza.Domain.Services.Interfaces.RenozaInterfaces
{
    /// <summary>
    /// Сервис для работы с ролями участников компаний
    /// </summary>
    public interface IMemberRoleService
    {
        /// <summary>
        /// Получить все роли
        /// </summary>
        /// <param name="includeInactive">Включать неактивные роли</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список ролей</returns>
        Task<Result<List<MemberRole>>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить роль по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор роли</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Роль</returns>
        Task<Result<MemberRole>> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        /// <summary>
        /// Получить роль по коду
        /// </summary>
        /// <param name="code">Код роли</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Роль</returns>
        Task<Result<MemberRole>> GetByCodeAsync(string code, CancellationToken cancellationToken);
    }
}
