using Renoza.Infrastructure.Contexts;
using Renoza.Infrastructure.Entities.Base;
using Renoza.Infrastructure.Repositories.Interfaces.BaseInterfaces;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Renoza.Infrastructure.Repositories.Implementations.BaseImplementations
{
    /// <summary>
    /// Базовый репозиторий работы с сущностями
    /// </summary>
    /// <typeparam name="T">Тип сущности</typeparam>
    public class EntityRepository<T> : IEntityRepository<T> where T : EntityDao
    {
        protected readonly BaseDbContext Context;

        /// <summary>
        /// Базовый репозиторий работы с сущностями
        /// </summary>
        /// <param name="context">Контекст работы с БД</param>
        public EntityRepository(BaseDbContext context)
        {
            Context = context;
        }

        /// <summary>
        /// Возвращает интерфейс для запроса
        /// </summary>
        /// <returns>Интерфейс для запроса</returns>
        public IQueryable<T> GetQueryable()
        {
            return Context.Set<T>();
        }

        /// <summary>
        /// Создает объект
        /// </summary>
        /// <param name="entity">Объект, который нужно создать</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Созданный объект</returns>
        public async Task<T> CreateAsync(T entity, CancellationToken cancellationToken)
        {
            var result = await Context.Set<T>().AddAsync(entity, cancellationToken);
            return result.Entity;
        }

        /// <summary>
        /// Добавляет список объектов
        /// </summary>
        /// <param name="entities">Список объектов, которые нужно добавить</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken)
        {
            await Context.Set<T>().AddRangeAsync(entities, cancellationToken);
        }

        /// <summary>
        /// Обновляет данные об объекте
        /// </summary>
        /// <param name="entity">Обновляемые данные об объекте</param>
        public void Update(T entity)
        {
            //Context.Entry(entity).State = EntityState.Modified;
            Context.Set<T>().Update(entity);
        }

        /// <summary>
        /// Обновляет список объектов
        /// </summary>
        /// <param name="entities">Список объектов, которые нужно обновить</param>
        public void UpdateRange(IEnumerable<T> entities)
        {
            Context.Set<T>().UpdateRange(entities);
        }

        /// <summary>
        /// Удаляет объект
        /// </summary>
        /// <param name="entity">Объект, который нужно удалить</param>
        public void Delete(T entity)
        {
            Context.Set<T>().Remove(entity);
        }

        /// <summary>
        /// Удаляет список объектов
        /// </summary>
        /// <param name="entities">Список объектов, которые нужно удалить</param>
        public void DeleteRange(IEnumerable<T> entities)
        {
            Context.Set<T>().RemoveRange(entities);
        }

        /// <summary>
        /// Удаляет список объектов по условию
        /// </summary>
        /// <param name="expression">Условие</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        public async Task DeleteByCondition(Expression<Func<T, bool>> expression, CancellationToken cancellationToken)
        {
            await Context.Set<T>().Where(expression).ExecuteDeleteAsync(cancellationToken);
        }

        #region IDisposable

        private bool _disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
