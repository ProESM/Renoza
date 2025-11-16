using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Renoza.Domain.Entities.Countries;
using Renoza.Domain.Services.Implementations.BaseImplementations;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;
using Renoza.Infrastructure.Contexts;
using Renoza.Infrastructure.Entities.Renoza;
using Renoza.Infrastructure.Repositories.Interfaces.BaseInterfaces;

namespace Renoza.Domain.Services.Implementations.RenozaImplementations
{
    /// <summary>
    /// Сервис работы с международными телефонными кодами
    /// </summary>
    public class PhoneCountryCodeService : BaseService<RenozaContext>, IPhoneCountryCodeService
    {
        #region Репозитории

        /// <summary>
        /// Репозиторий международных телефонных кодов
        /// </summary>
        private readonly IReadOnlyEntityWithIdRepository<PhoneCountryCodeDao, int> _phoneCountryCodeRepository;

        #endregion

        #region Мапперы

        /// <summary>
        /// Маппер для преобразования сущностей
        /// </summary>
        private readonly IMapper _mapper;

        #endregion

        /// <summary>
        /// Сервис работы с международными телефонными кодами
        /// </summary>
        /// <param name="dbContext">Контекст БД (Scoped, новый экземпляр для каждого запроса)</param>
        /// <param name="phoneCountryCodeRepository">Репозиторий международных телефонных кодов</param>
        /// <param name="mapper">Маппер для преобразования сущностей</param>
        public PhoneCountryCodeService(
            RenozaContext dbContext,
            IReadOnlyEntityWithIdRepository<PhoneCountryCodeDao, int> phoneCountryCodeRepository,
            IMapper mapper) : base(dbContext)
        {
            _phoneCountryCodeRepository = phoneCountryCodeRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Возвращает список всех международных телефонных кодов с информацией о странах
        /// </summary>
        /// <returns>Список международных телефонных кодов</returns>
        public async Task<List<PhoneCountryCode>> GetAllAsync()
        {
            var queryable = _phoneCountryCodeRepository.GetQueryable()
                .Include(x => x.Country)
                .OrderBy(x => x.Country.Name);

            return await _mapper.ProjectTo<PhoneCountryCode>(queryable).ToListAsync();
        }

        /// <summary>
        /// Возвращает список активных международных телефонных кодов
        /// </summary>
        /// <returns>Список активных международных телефонных кодов</returns>
        public async Task<List<PhoneCountryCode>> GetActiveAsync()
        {
            var queryable = _phoneCountryCodeRepository.GetQueryable()
                .Include(x => x.Country)
                .Where(x => x.Country.IsActive)
                .OrderBy(x => x.Country.Name);

            return await _mapper.ProjectTo<PhoneCountryCode>(queryable).ToListAsync();
        }
    }
}
