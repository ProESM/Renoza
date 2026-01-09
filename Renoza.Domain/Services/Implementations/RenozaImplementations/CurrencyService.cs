using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Renoza.Common.Base.Helpers;
using Renoza.Domain.Entities.Products;
using Renoza.Domain.Services.Implementations.BaseImplementations;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;
using Renoza.Infrastructure.Contexts;
using Renoza.Infrastructure.Entities.Renoza;
using Renoza.Infrastructure.Repositories.Interfaces.BaseInterfaces;

namespace Renoza.Domain.Services.Implementations.RenozaImplementations
{
    /// <summary>
    /// Сервис для работы со справочником валют
    /// </summary>
    public class CurrencyService : BaseService<RenozaContext>, ICurrencyService
    {
        #region Репозитории

        /// <summary>
        /// Репозиторий для работы с валютами
        /// </summary>
        private readonly IEntityWithIdRepository<CurrencyDao, Guid> _currencyRepository;

        #endregion

        #region Мапперы

        /// <summary>
        /// Маппер для преобразования между DAO и Domain сущностями
        /// </summary>
        private readonly IMapper _mapper;

        #endregion

        /// <summary>
        /// Сервис для работы со справочником валют
        /// </summary>
        /// <param name="context">Контекст базы данных</param>
        /// <param name="currencyRepository">Репозиторий для работы с валютами</param>
        /// <param name="mapper">Маппер для преобразования между DAO и Domain сущностями</param>
        public CurrencyService(
            RenozaContext context,
            IEntityWithIdRepository<CurrencyDao, Guid> currencyRepository,
            IMapper mapper) : base(context)
        {
            _currencyRepository = currencyRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Получить все валюты
        /// </summary>
        /// <param name="includeInactive">Включить неактивные валюты</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список валют</returns>
        public async Task<Result<List<Currency>>> GetAllAsync(
            bool includeInactive = false, CancellationToken cancellationToken = default)
        {
            var query = _currencyRepository.GetQueryable();

            if (!includeInactive)
                query = query.Where(x => x.IsActive);

            var currenciesDaos = await query
                .OrderBy(x => x.Code)
                .ToListAsync(cancellationToken);

            var currencies = _mapper.Map<List<Currency>>(currenciesDaos);
            return Result<List<Currency>>.Success(currencies);
        }

        /// <summary>
        /// Получить валюту по ID
        /// </summary>
        /// <param name="id">ID валюты</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Валюта</returns>
        public async Task<Result<Currency>> GetByIdAsync(
            Guid id, CancellationToken cancellationToken = default)
        {
            var currencyDao = await _currencyRepository.GetByIdAsync(id, cancellationToken);

            if (currencyDao == null)
                return Result<Currency>.Failure("Валюта не найдена");

            var currency = _mapper.Map<Currency>(currencyDao);
            return Result<Currency>.Success(currency);
        }

        /// <summary>
        /// Получить валюту по коду
        /// </summary>
        /// <param name="code">Код валюты (например, RUB, USD, EUR)</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Валюта</returns>
        public async Task<Result<Currency>> GetByCodeAsync(
            string code, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(code))
                return Result<Currency>.Failure("Код валюты не может быть пустым");

            var currencyDao = await _currencyRepository.GetQueryable()
                .FirstOrDefaultAsync(x => x.Code.ToLower() == code.ToLower(), cancellationToken);

            if (currencyDao == null)
                return Result<Currency>.Failure($"Валюта с кодом '{code}' не найдена");

            var currency = _mapper.Map<Currency>(currencyDao);
            return Result<Currency>.Success(currency);
        }
    }
}
