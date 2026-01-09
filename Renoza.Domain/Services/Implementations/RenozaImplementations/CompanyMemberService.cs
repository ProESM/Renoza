using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Renoza.Common.Base.Helpers;
using Renoza.Domain.Entities.CompanyMembers;
using Renoza.Domain.Services.Implementations.BaseImplementations;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;
using Renoza.Infrastructure.Contexts;
using Renoza.Infrastructure.Entities.Renoza;
using Renoza.Infrastructure.Repositories.Interfaces.BaseInterfaces;

namespace Renoza.Domain.Services.Implementations.RenozaImplementations
{
    /// <summary>
    /// Сервис для работы с участниками компании
    /// </summary>
    public class CompanyMemberService : BaseService<RenozaContext>, ICompanyMemberService
    {
        #region Репозитории

        /// <summary>
        /// Репозиторий для работы с участниками компаний
        /// </summary>
        private readonly IEntityWithIdRepository<CompanyMemberDao, Guid> _companyMemberRepository;

        #endregion

        #region Мапперы

        /// <summary>
        /// Маппер для преобразования между DAO и Domain сущностями
        /// </summary>
        private readonly IMapper _mapper;

        #endregion

        /// <summary>
        /// Сервис для работы с участниками компании
        /// </summary>
        /// <param name="context">Контекст базы данных</param>
        /// <param name="companyMemberRepository">Репозиторий для работы с участниками компаний</param>
        /// <param name="mapper">Маппер для преобразования между DAO и Domain сущностями</param>
        public CompanyMemberService(
            RenozaContext context,
            IEntityWithIdRepository<CompanyMemberDao, Guid> companyMemberRepository,
            IMapper mapper) : base(context)
        {
            _companyMemberRepository = companyMemberRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Добавить участника в компанию
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <param name="companyProfileId">ID профиля компании</param>
        /// <param name="memberRoleId">ID роли участника</param>
        /// <param name="position">Должность (опционально)</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Созданный участник компании</returns>
        public async Task<Result<CompanyMember>> AddMemberAsync(
            Guid userId,
            Guid companyProfileId,
            Guid memberRoleId,
            string? position = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Проверяем, не является ли пользователь уже участником этой компании
                var existingCompanyMemberDao = await _companyMemberRepository.GetQueryable()
                    .FirstOrDefaultAsync(cm => cm.UserId == userId && cm.CompanyProfileId == companyProfileId && cm.IsActive,
                        cancellationToken);

                if (existingCompanyMemberDao != null)
                {
                    return Result<CompanyMember>.Failure("Пользователь уже является участником этой компании");
                }

                var companyMemberDao = new CompanyMemberDao
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    CompanyProfileId = companyProfileId,
                    MemberRoleId = memberRoleId,
                    Position = position,
                    JoinedAt = DateTime.UtcNow,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _companyMemberRepository.CreateAsync(companyMemberDao, cancellationToken);
                await SaveChangesAsync(cancellationToken);

                return Result<CompanyMember>.Success(_mapper.Map<CompanyMember>(companyMemberDao));
            }
            catch (Exception ex)
            {
                return Result<CompanyMember>.Failure($"Ошибка при добавлении участника в компанию: {ex.Message}");
            }
        }

        /// <summary>
        /// Получить участника компании по ID пользователя и ID компании
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <param name="companyProfileId">ID профиля компании</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Участник компании</returns>
        public async Task<Result<CompanyMember>> GetMemberAsync(
            Guid userId,
            Guid companyProfileId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var companyMemberDao = await _companyMemberRepository.GetQueryable()
                    .FirstOrDefaultAsync(cm => cm.UserId == userId && cm.CompanyProfileId == companyProfileId && cm.IsActive,
                        cancellationToken);

                if (companyMemberDao == null)
                {
                    return Result<CompanyMember>.Failure("Участник компании не найден");
                }

                return Result<CompanyMember>.Success(_mapper.Map<CompanyMember>(companyMemberDao));
            }
            catch (Exception ex)
            {
                return Result<CompanyMember>.Failure($"Ошибка при получении участника компании: {ex.Message}");
            }
        }

        /// <summary>
        /// Получить всех активных участников компании
        /// </summary>
        /// <param name="companyProfileId">ID профиля компании</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список участников компании</returns>
        public async Task<Result<List<CompanyMember>>> GetCompanyMembersAsync(
            Guid companyProfileId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var companyMemberDaos = await _companyMemberRepository.GetQueryable()
                    .Where(cm => cm.CompanyProfileId == companyProfileId && cm.IsActive)
                    .ToListAsync(cancellationToken);

                return Result<List<CompanyMember>>.Success(_mapper.Map<List<CompanyMember>>(companyMemberDaos));
            }
            catch (Exception ex)
            {
                return Result<List<CompanyMember>>.Failure($"Ошибка при получении участников компании: {ex.Message}");
            }
        }

        /// <summary>
        /// Обновить роль участника в компании
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <param name="companyProfileId">ID профиля компании</param>
        /// <param name="newMemberRoleId">Новый ID роли участника</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Обновленный участник компании</returns>
        public async Task<Result<CompanyMember>> UpdateMemberRoleAsync(
            Guid userId,
            Guid companyProfileId,
            Guid newMemberRoleId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var companyMemberDao = await _companyMemberRepository.GetQueryable()
                    .FirstOrDefaultAsync(cm => cm.UserId == userId && cm.CompanyProfileId == companyProfileId && cm.IsActive,
                        cancellationToken);

                if (companyMemberDao == null)
                {
                    return Result<CompanyMember>.Failure("Участник компании не найден");
                }

                companyMemberDao.MemberRoleId = newMemberRoleId;
                companyMemberDao.UpdatedAt = DateTime.UtcNow;

                _companyMemberRepository.Update(companyMemberDao);
                await SaveChangesAsync(cancellationToken);

                return Result<CompanyMember>.Success(_mapper.Map<CompanyMember>(companyMemberDao));
            }
            catch (Exception ex)
            {
                return Result<CompanyMember>.Failure($"Ошибка при обновлении роли участника: {ex.Message}");
            }
        }

        /// <summary>
        /// Удалить участника из компании (мягкое удаление)
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <param name="companyProfileId">ID профиля компании</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат операции</returns>
        public async Task<Result<bool>> RemoveMemberAsync(
            Guid userId,
            Guid companyProfileId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var companyMemberDao = await _companyMemberRepository.GetQueryable()
                    .FirstOrDefaultAsync(cm => cm.UserId == userId && cm.CompanyProfileId == companyProfileId && cm.IsActive,
                        cancellationToken);

                if (companyMemberDao == null)
                {
                    return Result<bool>.Failure("Участник компании не найден");
                }

                companyMemberDao.IsActive = false;
                companyMemberDao.LeftAt = DateTime.UtcNow;
                companyMemberDao.UpdatedAt = DateTime.UtcNow;

                _companyMemberRepository.Update(companyMemberDao);
                await SaveChangesAsync(cancellationToken);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Ошибка при удалении участника из компании: {ex.Message}");
            }
        }
    }
}
