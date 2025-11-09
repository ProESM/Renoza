using Microsoft.EntityFrameworkCore;
using Renoza.Infrastructure.Contexts;
using Renoza.Infrastructure.Entities.Base;
using Renoza.Infrastructure.Repositories.Interfaces.BaseInterfaces;

namespace Renoza.Infrastructure.Repositories.Implementations.BaseImplementations
{
    /// <summary>
    /// Базовый репозиторий работы с сущностями с идентификатором
    /// </summary>
    /// <typeparam name="T">Тип сущности</typeparam>
    /// <typeparam name="TKey">Тип идентификатора сущности</typeparam>
    public class EntityWithIdRepository<T, TKey> : EntityRepository<T>, IEntityWithIdRepository<T, TKey> where T : EntityWithIdDao<TKey>
    {
        /// <summary>
        /// Базовый репозиторий работы с сущностями с идентификатором
        /// </summary>
        /// <param name="context">Контекст работы с БД</param>
        public EntityWithIdRepository(BaseDbContext context) : base(context)
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

        /// <summary>
        /// Возвращает объект по его идентификатору (только из БД)
        /// </summary>
        /// <param name="id">Идентификатор объекта</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Объект</returns>
        public async Task<T?> GetByIdFromDatabaseAsync(TKey id, CancellationToken cancellationToken)
        {
            var entity = Context.Set<T>().Local.FirstOrDefault(x => x.Id!.Equals(id));
            if (entity != null)
            {
                Context.Entry(entity).State = EntityState.Detached;
            }
            return await Context.Set<T>().FindAsync(id, cancellationToken);
        }
    }
}
