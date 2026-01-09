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
    /// Сервис для управления ценами профилей на товары/услуги с проверкой прав
    /// </summary>
    public class ProfileProductPriceService : BaseService<RenozaContext>, IProfileProductPriceService
    {
        #region Репозитории

        /// <summary>
        /// Репозиторий для работы с ценами
        /// </summary>
        private readonly IEntityRepository<ProfileProductPriceDao> _priceRepository;

        /// <summary>
        /// Репозиторий для работы с профилями TechnicalSupervisor
        /// </summary>
        private readonly IEntityWithIdRepository<TechnicalSupervisorProfileDao, Guid> _techSupervisorRepository;

        /// <summary>
        /// Репозиторий для работы с профилями Worker
        /// </summary>
        private readonly IEntityWithIdRepository<WorkerProfileDao, Guid> _workerRepository;

        /// <summary>
        /// Репозиторий для работы с участниками компаний
        /// </summary>
        private readonly IEntityWithIdRepository<CompanyMemberDao, Guid> _companyMemberRepository;

        /// <summary>
        /// Репозиторий для работы с товарами
        /// </summary>
        private readonly IEntityWithIdRepository<ProductDao, Guid> _productRepository;

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
        /// Сервис для управления ценами профилей на товары/услуги с проверкой прав
        /// </summary>
        /// <param name="context">Контекст базы данных</param>
        /// <param name="priceRepository">Репозиторий для работы с ценами</param>
        /// <param name="techSupervisorRepository">Репозиторий для работы с профилями TechnicalSupervisor</param>
        /// <param name="workerRepository">Репозиторий для работы с профилями Worker</param>
        /// <param name="companyMemberRepository">Репозиторий для работы с участниками компаний</param>
        /// <param name="productRepository">Репозиторий для работы с товарами</param>
        /// <param name="currencyRepository">Репозиторий для работы с валютами</param>
        /// <param name="mapper">Маппер для преобразования между DAO и Domain сущностями</param>
        public ProfileProductPriceService(
            RenozaContext context,
            IEntityRepository<ProfileProductPriceDao> priceRepository,
            IEntityWithIdRepository<TechnicalSupervisorProfileDao, Guid> techSupervisorRepository,
            IEntityWithIdRepository<WorkerProfileDao, Guid> workerRepository,
            IEntityWithIdRepository<CompanyMemberDao, Guid> companyMemberRepository,
            IEntityWithIdRepository<ProductDao, Guid> productRepository,
            IEntityWithIdRepository<CurrencyDao, Guid> currencyRepository,
            IMapper mapper) : base(context)
        {
            _priceRepository = priceRepository;
            _techSupervisorRepository = techSupervisorRepository;
            _workerRepository = workerRepository;
            _companyMemberRepository = companyMemberRepository;
            _productRepository = productRepository;
            _currencyRepository = currencyRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Установить цену для профиля на товар/услугу
        /// </summary>
        /// <param name="currentUserId">ID текущего пользователя (для проверки прав)</param>
        /// <param name="profileId">ID профиля (технадзор или рабочий)</param>
        /// <param name="productId">ID товара/услуги</param>
        /// <param name="currencyId">ID валюты</param>
        /// <param name="price">Цена</param>
        /// <param name="startDate">Дата начала действия цены</param>
        /// <param name="endDate">Дата окончания действия цены (null для бессрочной)</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Установленная цена</returns>
        public async Task<Result<ProfileProductPrice>> SetPriceAsync(
            Guid currentUserId, Guid profileId, Guid productId, Guid currencyId, decimal price,
            DateTime? startDate = null, DateTime? endDate = null,
            CancellationToken cancellationToken = default)
        {
            // Проверка прав доступа
            var permissionCheck = await CheckPermissionAsync(currentUserId, profileId, cancellationToken);
            if (!permissionCheck.IsSuccess)
                return Result<ProfileProductPrice>.Failure(permissionCheck.ErrorMessage!);

            // Проверка существования товара
            var productExists = await _productRepository.GetQueryable()
                .AnyAsync(x => x.Id == productId && x.IsActive, cancellationToken);

            if (!productExists)
                return Result<ProfileProductPrice>.Failure("Товар/услуга не найдена или неактивна");

            // Проверка существования валюты
            var currencyExists = await _currencyRepository.GetQueryable()
                .AnyAsync(x => x.Id == currencyId, cancellationToken);

            if (!currencyExists)
                return Result<ProfileProductPrice>.Failure("Валюта не найдена");

            if (price < 0)
                return Result<ProfileProductPrice>.Failure("Цена не может быть отрицательной");

            var effectiveStartDate = startDate ?? DateTime.UtcNow;
            var effectiveEndDate = endDate ?? DateTime.MaxValue;

            if (effectiveStartDate >= effectiveEndDate)
                return Result<ProfileProductPrice>.Failure("Дата начала должна быть раньше даты окончания");

            // Проверка на пересечение с существующими ценами
            var overlappingPrice = await _priceRepository.GetQueryable()
                .AnyAsync(x =>
                    x.ProfileId == profileId &&
                    x.ProductId == productId &&
                    x.StartDate < effectiveEndDate &&
                    x.EndDate > effectiveStartDate,
                    cancellationToken);

            if (overlappingPrice)
                return Result<ProfileProductPrice>.Failure("Период действия цены пересекается с существующей ценой");

            var priceDao = new ProfileProductPriceDao
            {
                ProfileId = profileId,
                ProductId = productId,
                StartDate = effectiveStartDate,
                EndDate = effectiveEndDate,
                CurrencyId = currencyId,
                Price = price
            };

            await _priceRepository.CreateAsync(priceDao, cancellationToken);
            await SaveChangesAsync(cancellationToken);

            var profileProductPrice = _mapper.Map<ProfileProductPrice>(priceDao);
            return Result<ProfileProductPrice>.Success(profileProductPrice);
        }

        /// <summary>
        /// Получить текущую цену для профиля на товар/услугу
        /// </summary>
        /// <param name="profileId">ID профиля</param>
        /// <param name="productId">ID товара/услуги</param>
        /// <param name="date">Дата, на которую нужна цена (null для текущей даты)</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Текущая цена</returns>
        public async Task<Result<ProfileProductPrice>> GetCurrentPriceAsync(
            Guid profileId, Guid productId, DateTime? date = null,
            CancellationToken cancellationToken = default)
        {
            var effectiveDate = date ?? DateTime.UtcNow;

            var priceDao = await _priceRepository.GetQueryable()
                .Where(x =>
                    x.ProfileId == profileId &&
                    x.ProductId == productId &&
                    x.StartDate <= effectiveDate &&
                    x.EndDate > effectiveDate)
                .OrderByDescending(x => x.StartDate)
                .FirstOrDefaultAsync(cancellationToken);

            if (priceDao == null)
                return Result<ProfileProductPrice>.Failure("Цена не найдена для указанной даты");

            var profileProductPrice = _mapper.Map<ProfileProductPrice>(priceDao);
            return Result<ProfileProductPrice>.Success(profileProductPrice);
        }

        /// <summary>
        /// Получить все цены для профиля
        /// </summary>
        /// <param name="profileId">ID профиля</param>
        /// <param name="includeExpired">Включить истекшие цены</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список цен</returns>
        public async Task<Result<List<ProfileProductPrice>>> GetPricesByProfileAsync(
            Guid profileId, bool includeExpired = false,
            CancellationToken cancellationToken = default)
        {
            var query = _priceRepository.GetQueryable()
                .Where(x => x.ProfileId == profileId);

            if (!includeExpired)
            {
                var now = DateTime.UtcNow;
                query = query.Where(x => x.EndDate > now);
            }

            var pricesDaos = await query
                .OrderBy(x => x.ProductId)
                .ThenByDescending(x => x.StartDate)
                .ToListAsync(cancellationToken);

            var prices = _mapper.Map<List<ProfileProductPrice>>(pricesDaos);
            return Result<List<ProfileProductPrice>>.Success(prices);
        }

        /// <summary>
        /// Получить историю цен для профиля на конкретный товар/услугу
        /// </summary>
        /// <param name="profileId">ID профиля</param>
        /// <param name="productId">ID товара/услуги</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>История цен</returns>
        public async Task<Result<List<ProfileProductPrice>>> GetPriceHistoryAsync(
            Guid profileId, Guid productId,
            CancellationToken cancellationToken = default)
        {
            var pricesDaos = await _priceRepository.GetQueryable()
                .Where(x => x.ProfileId == profileId && x.ProductId == productId)
                .OrderByDescending(x => x.StartDate)
                .ToListAsync(cancellationToken);

            var prices = _mapper.Map<List<ProfileProductPrice>>(pricesDaos);
            return Result<List<ProfileProductPrice>>.Success(prices);
        }

        /// <summary>
        /// Обновить цену для профиля на товар/услугу
        /// </summary>
        /// <param name="currentUserId">ID текущего пользователя (для проверки прав)</param>
        /// <param name="profileId">ID профиля</param>
        /// <param name="productId">ID товара/услуги</param>
        /// <param name="oldStartDate">Текущая дата начала действия цены</param>
        /// <param name="newPrice">Новая цена</param>
        /// <param name="newCurrencyId">Новая валюта</param>
        /// <param name="newEndDate">Новая дата окончания</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Обновленная цена</returns>
        public async Task<Result<ProfileProductPrice>> UpdatePriceAsync(
            Guid currentUserId, Guid profileId, Guid productId, DateTime oldStartDate,
            decimal? newPrice = null, Guid? newCurrencyId = null, DateTime? newEndDate = null,
            CancellationToken cancellationToken = default)
        {
            // Проверка прав доступа
            var permissionCheck = await CheckPermissionAsync(currentUserId, profileId, cancellationToken);
            if (!permissionCheck.IsSuccess)
                return Result<ProfileProductPrice>.Failure(permissionCheck.ErrorMessage!);

            var priceDao = await _priceRepository.GetQueryable()
                .FirstOrDefaultAsync(x =>
                    x.ProfileId == profileId &&
                    x.ProductId == productId &&
                    x.StartDate == oldStartDate,
                    cancellationToken);

            if (priceDao == null)
                return Result<ProfileProductPrice>.Failure("Цена не найдена");

            if (newPrice.HasValue)
            {
                if (newPrice.Value < 0)
                    return Result<ProfileProductPrice>.Failure("Цена не может быть отрицательной");

                priceDao.Price = newPrice.Value;
            }

            if (newCurrencyId.HasValue)
            {
                var currencyExists = await _currencyRepository.GetQueryable()
                    .AnyAsync(x => x.Id == newCurrencyId.Value, cancellationToken);

                if (!currencyExists)
                    return Result<ProfileProductPrice>.Failure("Валюта не найдена");

                priceDao.CurrencyId = newCurrencyId.Value;
            }

            if (newEndDate.HasValue)
            {
                if (priceDao.StartDate >= newEndDate.Value)
                    return Result<ProfileProductPrice>.Failure("Дата начала должна быть раньше даты окончания");

                priceDao.EndDate = newEndDate.Value;
            }

            _priceRepository.Update(priceDao);
            await SaveChangesAsync(cancellationToken);

            var profileProductPrice = _mapper.Map<ProfileProductPrice>(priceDao);
            return Result<ProfileProductPrice>.Success(profileProductPrice);
        }

        /// <summary>
        /// Завершить действие цены (установить EndDate на текущую дату)
        /// </summary>
        /// <param name="currentUserId">ID текущего пользователя (для проверки прав)</param>
        /// <param name="profileId">ID профиля</param>
        /// <param name="productId">ID товара/услуги</param>
        /// <param name="startDate">Дата начала действия цены</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат операции</returns>
        public async Task<Result<bool>> TerminatePriceAsync(
            Guid currentUserId, Guid profileId, Guid productId, DateTime startDate,
            CancellationToken cancellationToken = default)
        {
            // Проверка прав доступа
            var permissionCheck = await CheckPermissionAsync(currentUserId, profileId, cancellationToken);
            if (!permissionCheck.IsSuccess)
                return Result<bool>.Failure(permissionCheck.ErrorMessage!);

            var priceDao = await _priceRepository.GetQueryable()
                .FirstOrDefaultAsync(x =>
                    x.ProfileId == profileId &&
                    x.ProductId == productId &&
                    x.StartDate == startDate,
                    cancellationToken);

            if (priceDao == null)
                return Result<bool>.Failure("Цена не найдена");

            priceDao.EndDate = DateTime.UtcNow;

            _priceRepository.Update(priceDao);
            await SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }

        /// <summary>
        /// Удалить цену для профиля на товар/услугу
        /// </summary>
        /// <param name="currentUserId">ID текущего пользователя (для проверки прав)</param>
        /// <param name="profileId">ID профиля</param>
        /// <param name="productId">ID товара/услуги</param>
        /// <param name="startDate">Дата начала действия цены</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат удаления</returns>
        public async Task<Result<bool>> DeletePriceAsync(
            Guid currentUserId, Guid profileId, Guid productId, DateTime startDate,
            CancellationToken cancellationToken = default)
        {
            // Проверка прав доступа
            var permissionCheck = await CheckPermissionAsync(currentUserId, profileId, cancellationToken);
            if (!permissionCheck.IsSuccess)
                return Result<bool>.Failure(permissionCheck.ErrorMessage!);

            var priceDao = await _priceRepository.GetQueryable()
                .FirstOrDefaultAsync(x =>
                    x.ProfileId == profileId &&
                    x.ProductId == productId &&
                    x.StartDate == startDate,
                    cancellationToken);

            if (priceDao == null)
                return Result<bool>.Failure("Цена не найдена");

            _priceRepository.Delete(priceDao);
            await SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }

        /// <summary>
        /// Проверить, имеет ли текущий пользователь право управлять ценами для указанного профиля
        /// </summary>
        /// <param name="currentUserId">ID текущего пользователя</param>
        /// <param name="targetProfileId">ID профиля, для которого нужно управлять ценами</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат проверки прав</returns>
        private async Task<Result<bool>> CheckPermissionAsync(
            Guid currentUserId, Guid targetProfileId, CancellationToken cancellationToken)
        {
            // Получаем UserId владельца профиля
            Guid? profileOwnerId = null;

            // Проверяем в TechnicalSupervisorProfiles
            var techSupervisor = await _techSupervisorRepository.GetByIdAsync(targetProfileId, cancellationToken);
            if (techSupervisor != null)
            {
                profileOwnerId = techSupervisor.UserId;
            }
            else
            {
                // Проверяем в WorkerProfiles
                var worker = await _workerRepository.GetByIdAsync(targetProfileId, cancellationToken);
                if (worker != null)
                {
                    profileOwnerId = worker.UserId;
                }
            }

            if (!profileOwnerId.HasValue)
                return Result<bool>.Failure("Профиль не найден");

            // Находим компанию владельца профиля
            var profileOwnerMembership = await _companyMemberRepository.GetQueryable()
                .Where(x => x.UserId == profileOwnerId.Value && x.IsActive)
                .Include(x => x.MemberRole)
                .FirstOrDefaultAsync(cancellationToken);

            if (profileOwnerMembership == null)
                return Result<bool>.Failure("Профиль не привязан к компании");

            var companyProfileId = profileOwnerMembership.CompanyProfileId;

            // Проверяем, что текущий пользователь является участником той же компании
            var currentUserMembership = await _companyMemberRepository.GetQueryable()
                .Where(x =>
                    x.UserId == currentUserId &&
                    x.CompanyProfileId == companyProfileId &&
                    x.IsActive)
                .Include(x => x.MemberRole)
                .FirstOrDefaultAsync(cancellationToken);

            if (currentUserMembership == null)
                return Result<bool>.Failure("Вы не являетесь участником компании, к которой относится этот профиль");

            // Проверяем роль: только Owner и Manager могут управлять ценами
            var roleCode = currentUserMembership.MemberRole.Code.ToLowerInvariant();
            if (roleCode != "owner" && roleCode != "manager")
                return Result<bool>.Failure("Недостаточно прав для управления ценами. Требуется роль Owner или Manager");

            return Result<bool>.Success(true);
        }
    }
}
