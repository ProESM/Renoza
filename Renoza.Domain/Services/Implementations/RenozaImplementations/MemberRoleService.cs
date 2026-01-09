using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Renoza.Common.Base.Helpers;
using Renoza.Domain.Entities.CompanyMembers;
using Renoza.Domain.Services.Implementations.BaseImplementations;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;
using Renoza.Infrastructure.Contexts;
using Renoza.Infrastructure.Entities.Renoza;

namespace Renoza.Domain.Services.Implementations.RenozaImplementations
{
    /// <summary>
    /// Сервис для работы с ролями участников компаний
    /// </summary>
    public class MemberRoleService : BaseService<RenozaContext>, IMemberRoleService
    {
        #region Мапперы

        /// <summary>
        /// Маппер для преобразования между DAO и Domain сущностями
        /// </summary>
        private readonly IMapper _mapper;

        #endregion

        /// <summary>
        /// Сервис для работы с ролями участников компаний
        /// </summary>
        /// <param name="context">Контекст базы данных</param>
        /// <param name="mapper">Маппер для преобразования между DAO и Domain сущностями</param>
        public MemberRoleService(RenozaContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }

        /// <summary>
        /// Получить все роли
        /// </summary>
        /// <param name="includeInactive">Включать неактивные роли</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список ролей</returns>
        public async Task<Result<List<MemberRole>>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
        {
            try
            {
                var query = Context.Set<MemberRoleDao>().AsQueryable();

                if (!includeInactive)
                {
                    query = query.Where(x => x.IsActive);
                }

                var roles = await query
                    .OrderBy(x => x.Name)
                    .ToListAsync(cancellationToken);

                var result = _mapper.Map<List<MemberRole>>(roles);
                return Result<List<MemberRole>>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<List<MemberRole>>.Failure($"Ошибка при получении списка ролей: {ex.Message}");
            }
        }

        /// <summary>
        /// Получить роль по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор роли</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Роль</returns>
        public async Task<Result<MemberRole>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var roleDao = await Context.Set<MemberRoleDao>()
                    .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

                if (roleDao == null)
                {
                    return Result<MemberRole>.Failure("Роль не найдена");
                }

                var result = _mapper.Map<MemberRole>(roleDao);
                return Result<MemberRole>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<MemberRole>.Failure($"Ошибка при получении роли: {ex.Message}");
            }
        }

        /// <summary>
        /// Получить роль по коду
        /// </summary>
        /// <param name="code">Код роли</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Роль</returns>
        public async Task<Result<MemberRole>> GetByCodeAsync(string code, CancellationToken cancellationToken)
        {
            try
            {
                var roleDao = await Context.Set<MemberRoleDao>()
                    .FirstOrDefaultAsync(x => x.Code == code, cancellationToken);

                if (roleDao == null)
                {
                    return Result<MemberRole>.Failure("Роль не найдена");
                }

                var result = _mapper.Map<MemberRole>(roleDao);
                return Result<MemberRole>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<MemberRole>.Failure($"Ошибка при получении роли: {ex.Message}");
            }
        }
    }
}
