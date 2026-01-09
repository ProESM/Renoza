using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Renoza.Common.Base.Helpers;
using Renoza.Domain.Entities.CompanyProfiles;
using Renoza.Domain.Services.Implementations.BaseImplementations;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;
using Renoza.Infrastructure.Contexts;
using Renoza.Infrastructure.Entities.Renoza;
using Renoza.Infrastructure.Repositories.Interfaces.BaseInterfaces;

namespace Renoza.Domain.Services.Implementations.RenozaImplementations
{
    /// <summary>
    /// Сервис для работы с профилями компаний
    /// </summary>
    public class CompanyProfileService : BaseService<RenozaContext>, ICompanyProfileService
    {
        #region Репозитории

        /// <summary>
        /// Репозиторий для работы с профилями компаний
        /// </summary>
        private readonly IEntityWithIdRepository<CompanyProfileDao, Guid> _companyProfileRepository;

        #endregion

        #region Мапперы

        /// <summary>
        /// Маппер для преобразования между DAO и Domain сущностями
        /// </summary>
        private readonly IMapper _mapper;

        #endregion

        /// <summary>
        /// Сервис для работы с профилями компаний
        /// </summary>
        /// <param name="context">Контекст базы данных</param>
        /// <param name="companyProfileRepository">Репозиторий для работы с профилями компаний</param>
        /// <param name="mapper">Маппер для преобразования между DAO и Domain сущностями</param>
        public CompanyProfileService(
            RenozaContext context,
            IEntityWithIdRepository<CompanyProfileDao, Guid> companyProfileRepository,
            IMapper mapper) : base(context)
        {
            _companyProfileRepository = companyProfileRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Создать профиль компании
        /// </summary>
        /// <param name="inn">ИНН компании</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Созданный профиль компании</returns>
        public async Task<Result<CompanyProfile>> CreateCompanyProfileAsync(string inn,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Проверяем, нет ли уже профиля с таким ИНН
                var existingCompanyProfileDao = await _companyProfileRepository.GetQueryable()
                    .FirstOrDefaultAsync(cp => cp.Inn == inn, cancellationToken);

                if (existingCompanyProfileDao != null)
                {
                    return Result<CompanyProfile>.Success(_mapper.Map<CompanyProfile>(existingCompanyProfileDao));
                }

                var companyProfileDao = new CompanyProfileDao
                {
                    Id = Guid.NewGuid(),
                    Inn = inn,
                    IsCompanyVerified = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _companyProfileRepository.CreateAsync(companyProfileDao, cancellationToken);
                await SaveChangesAsync(cancellationToken);

                return Result<CompanyProfile>.Success(_mapper.Map<CompanyProfile>(companyProfileDao));
            }
            catch (Exception ex)
            {
                return Result<CompanyProfile>.Failure($"Ошибка при создании профиля компании: {ex.Message}");
            }
        }

        /// <summary>
        /// Получить профиль компании по ID
        /// </summary>
        /// <param name="id">ID профиля компании</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Профиль компании</returns>
        public async Task<Result<CompanyProfile>> GetCompanyProfileByIdAsync(Guid id,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var companyProfileDao = await _companyProfileRepository.GetByIdAsync(id, cancellationToken);

                if (companyProfileDao == null)
                {
                    return Result<CompanyProfile>.Failure("Профиль компании не найден");
                }

                return Result<CompanyProfile>.Success(_mapper.Map<CompanyProfile>(companyProfileDao));
            }
            catch (Exception ex)
            {
                return Result<CompanyProfile>.Failure($"Ошибка при получении профиля компании: {ex.Message}");
            }
        }

        /// <summary>
        /// Получить профиль компании по ИНН
        /// </summary>
        /// <param name="inn">ИНН компании</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Профиль компании</returns>
        public async Task<Result<CompanyProfile>> GetCompanyProfileByInnAsync(string inn,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var companyProfileDao = await _companyProfileRepository.GetQueryable()
                    .FirstOrDefaultAsync(cp => cp.Inn == inn, cancellationToken);

                if (companyProfileDao == null)
                {
                    return Result<CompanyProfile>.Failure("Профиль компании не найден");
                }

                return Result<CompanyProfile>.Success(_mapper.Map<CompanyProfile>(companyProfileDao));
            }
            catch (Exception ex)
            {
                return Result<CompanyProfile>.Failure($"Ошибка при получении профиля компании: {ex.Message}");
            }
        }

        /// <summary>
        /// Обновить статус верификации компании и продублировать ключевые поля
        /// </summary>
        /// <param name="companyProfileId">ID профиля компании</param>
        /// <param name="companyVerificationId">ID результата верификации</param>
        /// <param name="isVerified">Статус верификации</param>
        /// <param name="companyType">Тип компании (0 - неизвестно, 1 - юр.лицо, 2 - ИП)</param>
        /// <param name="updateData">Дублированные поля для обновления профиля</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат операции</returns>
        public async Task<Result<CompanyProfile>> UpdateVerificationStatusAsync(
            Guid companyProfileId,
            Guid companyVerificationId,
            bool isVerified,
            short companyType,
            CompanyProfileUpdateData? updateData = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var companyProfileDao = await _companyProfileRepository.GetByIdAsync(companyProfileId, cancellationToken);

                if (companyProfileDao == null)
                {
                    return Result<CompanyProfile>.Failure("Профиль компании не найден");
                }

                companyProfileDao.CompanyVerificationId = companyVerificationId;
                companyProfileDao.IsCompanyVerified = isVerified;
                companyProfileDao.CompanyVerifiedAt = isVerified ? DateTime.UtcNow : null;
                companyProfileDao.CompanyTypeId = companyType;
                companyProfileDao.UpdatedAt = DateTime.UtcNow;

                // Дублируем ключевые поля из верификации для оптимизации производительности
                if (updateData != null)
                {
                    companyProfileDao.FullName = updateData.FullName;
                    companyProfileDao.ShortName = updateData.ShortName;
                    companyProfileDao.Ogrn = updateData.Ogrn;
                    companyProfileDao.Kpp = updateData.Kpp;
                    companyProfileDao.Address = updateData.Address;
                    companyProfileDao.DirectorName = updateData.DirectorName;
                    companyProfileDao.RegistrationDate = updateData.RegistrationDate;
                    companyProfileDao.Status = updateData.Status;
                }

                _companyProfileRepository.Update(companyProfileDao);
                await SaveChangesAsync(cancellationToken);

                return Result<CompanyProfile>.Success(_mapper.Map<CompanyProfile>(companyProfileDao));
            }
            catch (Exception ex)
            {
                return Result<CompanyProfile>.Failure($"Ошибка при обновлении статуса верификации: {ex.Message}");
            }
        }
    }
}
