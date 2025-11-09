using Renoza.Infrastructure.Contexts;
using Renoza.Infrastructure.Entities.Base;
using Renoza.Infrastructure.Repositories.Interfaces.BaseInterfaces;

namespace Renoza.Infrastructure.Repositories.Implementations.BaseImplementations
{
    /// <summary>
    /// Базовый репозиторий работы с сущностями с идентификаторами в режиме "только чтение"
    /// </summary>
    /// <typeparam name="T">Тип сущности</typeparam>
    /// <typeparam name="TKey">Тип идентификатора сущности</typeparam>
    public class ReadOnlyEntityWithIdRepository<T, TKey> : ReadOnlyEntityRepository<T>, IReadOnlyEntityWithIdRepository<T, TKey> where T : EntityWithIdDao<TKey>
    {
        /// <summary>
        /// Базовый репозиторий работы с сущностями с идентификаторами в режиме "только чтение"
        /// </summary>
        /// <param name="context">Контекст работы с БД</param>
        public ReadOnlyEntityWithIdRepository(BaseDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Возвращает объект по его идентификатору
        /// </summary>
        /// <param name="id">Идентификатор объекта</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Объект</returns>
        public async Task<T?> GetByIdAsync(TKey id, CancellationToken cancellationToken)
        {
            return await Context.Set<T>().FindAsync(new object[] { id }, cancellationToken);
        }
    }
}
