using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Renoza.Domain.Entities.Profiles;
using Renoza.Domain.Services.Implementations.BaseImplementations;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;
using Renoza.Infrastructure.Contexts;
using Renoza.Infrastructure.Entities.Renoza;
using Renoza.Infrastructure.Repositories.Interfaces.BaseInterfaces;

namespace Renoza.Domain.Services.Implementations.RenozaImplementations
{
    /// <summary>
    /// Сервис работы с профилями работников
    /// </summary>
    public class WorkerProfileService : BaseService<RenozaContext>, IWorkerProfileService
    {
        #region Репозитории

        /// <summary>
        /// Репозиторий профилей работников
        /// </summary>
        private readonly IEntityWithIdRepository<WorkerProfileDao, Guid> _workerProfileRepository;

        #endregion

        #region Мапперы

        /// <summary>
        /// Маппер для преобразования сущностей
        /// </summary>
        private readonly IMapper _mapper;

        #endregion

        /// <summary>
        /// Сервис работы с профилями работников
        /// </summary>
        /// <param name="dbContext">Контекст БД</param>
        /// <param name="workerProfileRepository">Репозиторий профилей работников</param>
        /// <param name="mapper">Маппер для преобразования сущностей</param>
        public WorkerProfileService(
            RenozaContext dbContext,
            IEntityWithIdRepository<WorkerProfileDao, Guid> workerProfileRepository,
            IMapper mapper) : base(dbContext)
        {
            _workerProfileRepository = workerProfileRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Возвращает интерфейс для запроса профилей работников
        /// </summary>
        /// <returns>Интерфейс для запроса профилей работников</returns>
        public IQueryable<WorkerProfile> GetQueryable()
        {
            var queryable = _workerProfileRepository.GetQueryable();
            return _mapper.ProjectTo<WorkerProfile>(queryable);
        }

        /// <summary>
        /// Получить профиль работника по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор профиля</param>
        /// <returns>Профиль работника</returns>
        public async Task<WorkerProfile?> GetByIdAsync(Guid id)
        {
            var profileDao = await _workerProfileRepository.GetByIdAsync(id, CancellationToken.None);
            return profileDao != null ? _mapper.Map<WorkerProfile>(profileDao) : null;
        }

        /// <summary>
        /// Получить профиль работника по идентификатору пользователя
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns>Профиль работника</returns>
        public async Task<WorkerProfile?> GetByUserIdAsync(Guid userId)
        {
            var profileDao = await _workerProfileRepository.GetQueryable()
                .FirstOrDefaultAsync(p => p.UserId == userId);
            return profileDao != null ? _mapper.Map<WorkerProfile>(profileDao) : null;
        }

        /// <summary>
        /// Создать профиль работника
        /// </summary>
        /// <param name="profile">Профиль работника</param>
        /// <returns>Созданный профиль</returns>
        public async Task<WorkerProfile> CreateAsync(WorkerProfile profile)
        {
            var profileDao = new WorkerProfileDao
            {
                Id = Guid.NewGuid(),
                UserId = profile.UserId,
                Specialization = profile.Specialization,
                TeamSize = profile.TeamSize,
                Certifications = profile.Certifications,
                ProfessionalStartDate = profile.ProfessionalStartDate,
                IsAvailable = true,
                Rating = profile.Rating,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _workerProfileRepository.CreateAsync(profileDao, CancellationToken.None);
            await SaveChangesAsync();
            return _mapper.Map<WorkerProfile>(profileDao);
        }

        /// <summary>
        /// Обновить профиль работника
        /// </summary>
        /// <param name="profile">Профиль работника</param>
        /// <returns>Обновленный профиль</returns>
        public async Task<WorkerProfile> UpdateAsync(WorkerProfile profile)
        {
            var profileDao = await _workerProfileRepository.GetByIdAsync(profile.Id, CancellationToken.None);
            if (profileDao == null)
                throw new InvalidOperationException($"Профиль работника с Id {profile.Id} не найден");

            profileDao.Specialization = profile.Specialization;
            profileDao.TeamSize = profile.TeamSize;
            profileDao.Certifications = profile.Certifications;
            profileDao.ProfessionalStartDate = profile.ProfessionalStartDate;
            profileDao.Rating = profile.Rating;
            profileDao.UpdatedAt = DateTime.UtcNow;

            _workerProfileRepository.Update(profileDao);
            await SaveChangesAsync();
            return _mapper.Map<WorkerProfile>(profileDao);
        }

        /// <summary>
        /// Деактивировать профиль работника
        /// </summary>
        /// <param name="id">Идентификатор профиля</param>
        /// <returns>Успешность операции</returns>
        public async Task<bool> DeactivateAsync(Guid id)
        {
            var profileDao = await _workerProfileRepository.GetByIdAsync(id, CancellationToken.None);
            if (profileDao == null)
                return false;

            profileDao.IsActive = false;
            profileDao.UpdatedAt = DateTime.UtcNow;
            _workerProfileRepository.Update(profileDao);
            await SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Установить доступность работника
        /// </summary>
        /// <param name="id">Идентификатор профиля</param>
        /// <param name="isAvailable">Доступность</param>
        /// <returns>Успешность операции</returns>
        public async Task<bool> SetAvailabilityAsync(Guid id, bool isAvailable)
        {
            var profileDao = await _workerProfileRepository.GetByIdAsync(id, CancellationToken.None);
            if (profileDao == null)
                return false;

            profileDao.IsAvailable = isAvailable;
            profileDao.UpdatedAt = DateTime.UtcNow;
            _workerProfileRepository.Update(profileDao);
            await SaveChangesAsync();
            return true;
        }
    }
}
