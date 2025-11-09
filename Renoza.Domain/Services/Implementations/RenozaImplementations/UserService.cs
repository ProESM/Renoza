using AutoMapper;
using Renoza.Domain.Entities.Users;
using Renoza.Domain.Services.Implementations.BaseImplementations;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;
using Renoza.Infrastructure.Contexts;
using Renoza.Infrastructure.Entities.Renoza;
using Renoza.Infrastructure.Repositories.Interfaces.BaseInterfaces;

namespace Renoza.Domain.Services.Implementations.RenozaImplementations
{
    /// <summary>
    /// Сервис работы с пользователями
    /// </summary>
    public class UserService : BaseService, IUserService
    {
        #region Контексты

        /// <summary>
        /// Контекст БД
        /// </summary>
        private readonly RenozaContext _dbContext;

        #endregion

        #region Репозитории

        /// <summary>
        /// Репозиторий пользователей
        /// </summary>
        private readonly IEntityWithIdRepository<UserDao, int> _userRepository;

        #endregion

        #region Мапперы

        /// <summary>
        /// Маппер для преобразования сущностей
        /// </summary>
        private readonly IMapper _mapper;

        #endregion

        /// <summary>
        /// Сервис работы с пользователями
        /// </summary>
        /// <param name="dbContext">Контекст БД (Scoped, новый экземпляр для каждого запроса)</param>
        /// <param name="productRepository">Репозиторий продуктов</param>
        /// <param name="mapper">Маппер для преобразования сущностей</param>
        public UserService(
            RenozaContext dbContext,
            IEntityWithIdRepository<UserDao, int> userRepository,
            IMapper mapper) : base(dbContext)
        {
            _dbContext = dbContext;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Возвращает интерфейс для запроса пользователей
        /// </summary>
        /// <returns>Интерфейс для запроса пользователей</returns>
        public IQueryable<User> GetQueryable()
        {
            var queryable = _userRepository.GetQueryable();
            return _mapper.ProjectTo<User>(queryable);
        }
    }
}
