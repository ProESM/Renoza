using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Renoza.Domain.Constants.Auth;
using Renoza.Domain.Entities.Roles;
using Renoza.Domain.Services.Implementations.BaseImplementations;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;
using Renoza.Infrastructure.Contexts;
using Renoza.Infrastructure.Entities.Renoza;
using Renoza.Infrastructure.Repositories.Interfaces.BaseInterfaces;

namespace Renoza.Domain.Services.Implementations.RenozaImplementations
{
    /// <summary>
    /// Сервис работы с ролями
    /// </summary>
    public class RoleService : BaseService<RenozaContext>, IRoleService
    {
        #region Репозитории

        /// <summary>
        /// Репозиторий ролей
        /// </summary>
        private readonly IEntityWithIdRepository<RoleDao, Guid> _roleRepository;

        /// <summary>
        /// Репозиторий связей пользователей и ролей
        /// </summary>
        private readonly IEntityRepository<UserRoleDao> _userRoleRepository;

        #endregion

        #region Мапперы

        /// <summary>
        /// Маппер для преобразования сущностей
        /// </summary>
        private readonly IMapper _mapper;

        #endregion

        /// <summary>
        /// Сервис работы с ролями
        /// </summary>
        /// <param name="dbContext">Контекст БД</param>
        /// <param name="roleRepository">Репозиторий ролей</param>
        /// <param name="userRoleRepository">Репозиторий связей пользователей и ролей</param>
        /// <param name="mapper">Маппер для преобразования сущностей</param>
        public RoleService(
            RenozaContext dbContext,
            IEntityWithIdRepository<RoleDao, Guid> roleRepository,
            IEntityRepository<UserRoleDao> userRoleRepository,
            IMapper mapper) : base(dbContext)
        {
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Возвращает интерфейс для запроса ролей
        /// </summary>
        /// <returns>Интерфейс для запроса ролей</returns>
        public IQueryable<Role> GetQueryable()
        {
            var queryable = _roleRepository.GetQueryable();
            return _mapper.ProjectTo<Role>(queryable);
        }

        /// <summary>
        /// Получить роль по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор роли</param>
        /// <returns>Роль</returns>
        public async Task<Role?> GetByIdAsync(Guid id)
        {
            var roleDao = await _roleRepository.GetByIdAsync(id, CancellationToken.None);
            return roleDao != null ? _mapper.Map<Role>(roleDao) : null;
        }

        /// <summary>
        /// Получить идентификатор роли по имени
        /// </summary>
        /// <param name="roleName">Имя роли (Administrator, Customer, Worker, TechnicalSupervisor)</param>
        /// <returns>Идентификатор роли или null</returns>
        public Guid? GetRoleIdByName(string roleName)
        {
            var field = typeof(RoleIds).GetField(roleName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

            if (field == null || field.FieldType != typeof(Guid))
                return null;

            return (Guid?)field.GetValue(null);
        }

        /// <summary>
        /// Получить роли пользователя
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns>Список ролей пользователя</returns>
        public async Task<List<Role>> GetUserRolesAsync(Guid userId)
        {
            var userRoles = await _userRoleRepository.GetQueryable()
                .Where(ur => ur.UserId == userId && ur.IsActive)
                .Include(ur => ur.Role)
                .Select(ur => ur.Role)
                .ToListAsync();

            return _mapper.Map<List<Role>>(userRoles);
        }

        /// <summary>
        /// Назначить роль пользователю
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="roleId">Идентификатор роли</param>
        /// <returns>Успешность операции</returns>
        public async Task<bool> AssignRoleToUserAsync(Guid userId, Guid roleId)
        {
            var existingUserRole = await _userRoleRepository.GetQueryable()
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

            if (existingUserRole != null)
            {
                if (!existingUserRole.IsActive)
                {
                    existingUserRole.IsActive = true;
                    existingUserRole.UpdatedAt = DateTime.UtcNow;
                    _userRoleRepository.Update(existingUserRole);
                    await SaveChangesAsync();
                }
                return true;
            }

            var userRole = new UserRoleDao
            {
                UserId = userId,
                RoleId = roleId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _userRoleRepository.CreateAsync(userRole, CancellationToken.None);
            await SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Отозвать роль у пользователя
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="roleId">Идентификатор роли</param>
        /// <returns>Успешность операции</returns>
        public async Task<bool> RevokeRoleFromUserAsync(Guid userId, Guid roleId)
        {
            var userRole = await _userRoleRepository.GetQueryable()
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

            if (userRole == null)
                return false;

            userRole.IsActive = false;
            userRole.UpdatedAt = DateTime.UtcNow;
            _userRoleRepository.Update(userRole);
            await SaveChangesAsync();
            return true;
        }
    }
}
