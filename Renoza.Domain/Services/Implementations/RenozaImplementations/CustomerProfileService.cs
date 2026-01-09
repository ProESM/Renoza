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
    /// Сервис работы с профилями заказчиков
    /// </summary>
    public class CustomerProfileService : BaseService<RenozaContext>, ICustomerProfileService
    {
        #region Репозитории

        /// <summary>
        /// Репозиторий профилей заказчиков
        /// </summary>
        private readonly IEntityWithIdRepository<CustomerProfileDao, Guid> _customerProfileRepository;

        #endregion

        #region Мапперы

        /// <summary>
        /// Маппер для преобразования сущностей
        /// </summary>
        private readonly IMapper _mapper;

        #endregion

        /// <summary>
        /// Сервис работы с профилями заказчиков
        /// </summary>
        /// <param name="context">Контекст БД</param>
        /// <param name="customerProfileRepository">Репозиторий профилей заказчиков</param>
        /// <param name="mapper">Маппер для преобразования сущностей</param>
        public CustomerProfileService(
            RenozaContext context,
            IEntityWithIdRepository<CustomerProfileDao, Guid> customerProfileRepository,
            IMapper mapper) : base(context)
        {
            _customerProfileRepository = customerProfileRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Возвращает интерфейс для запроса профилей заказчиков
        /// </summary>
        /// <returns>Интерфейс для запроса профилей заказчиков</returns>
        public IQueryable<CustomerProfile> GetQueryable()
        {
            var queryable = _customerProfileRepository.GetQueryable();
            return _mapper.ProjectTo<CustomerProfile>(queryable);
        }

        /// <summary>
        /// Получить профиль заказчика по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор профиля</param>
        /// <returns>Профиль заказчика</returns>
        public async Task<CustomerProfile?> GetByIdAsync(Guid id)
        {
            var profileDao = await _customerProfileRepository.GetByIdAsync(id, CancellationToken.None);
            return profileDao != null ? _mapper.Map<CustomerProfile>(profileDao) : null;
        }

        /// <summary>
        /// Получить профиль заказчика по идентификатору пользователя
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns>Профиль заказчика</returns>
        public async Task<CustomerProfile?> GetByUserIdAsync(Guid userId)
        {
            var profileDao = await _customerProfileRepository.GetQueryable()
                .FirstOrDefaultAsync(p => p.UserId == userId);
            return profileDao != null ? _mapper.Map<CustomerProfile>(profileDao) : null;
        }

        /// <summary>
        /// Создать профиль заказчика
        /// </summary>
        /// <param name="profile">Профиль заказчика</param>
        /// <returns>Созданный профиль</returns>
        public async Task<CustomerProfile> CreateAsync(CustomerProfile profile)
        {
            var profileDao = new CustomerProfileDao
            {
                Id = Guid.NewGuid(),
                UserId = profile.UserId,
                CompanyName = profile.CompanyName,
                TaxId = profile.TaxId,
                BillingAddress = profile.BillingAddress,
                CreditLimit = profile.CreditLimit,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _customerProfileRepository.CreateAsync(profileDao, CancellationToken.None);
            await SaveChangesAsync();
            return _mapper.Map<CustomerProfile>(profileDao);
        }

        /// <summary>
        /// Обновить профиль заказчика
        /// </summary>
        /// <param name="profile">Профиль заказчика</param>
        /// <returns>Обновленный профиль</returns>
        public async Task<CustomerProfile> UpdateAsync(CustomerProfile profile)
        {
            var profileDao = await _customerProfileRepository.GetByIdAsync(profile.Id, CancellationToken.None);
            if (profileDao == null)
                throw new InvalidOperationException($"Профиль заказчика с Id {profile.Id} не найден");

            profileDao.CompanyName = profile.CompanyName;
            profileDao.TaxId = profile.TaxId;
            profileDao.BillingAddress = profile.BillingAddress;
            profileDao.CreditLimit = profile.CreditLimit;
            profileDao.UpdatedAt = DateTime.UtcNow;

            _customerProfileRepository.Update(profileDao);
            await SaveChangesAsync();
            return _mapper.Map<CustomerProfile>(profileDao);
        }

        /// <summary>
        /// Деактивировать профиль заказчика
        /// </summary>
        /// <param name="id">Идентификатор профиля</param>
        /// <returns>Успешность операции</returns>
        public async Task<bool> DeactivateAsync(Guid id)
        {
            var profileDao = await _customerProfileRepository.GetByIdAsync(id, CancellationToken.None);
            if (profileDao == null)
                return false;

            profileDao.IsActive = false;
            profileDao.UpdatedAt = DateTime.UtcNow;
            _customerProfileRepository.Update(profileDao);
            await SaveChangesAsync();
            return true;
        }
    }
}
