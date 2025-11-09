using Renoza.Infrastructure.Contexts;
using Renoza.Infrastructure.Entities.Base;
using Renoza.Infrastructure.Repositories.Interfaces.BaseInterfaces;

namespace Renoza.Infrastructure.Repositories.Implementations.BaseImplementations
{
    /// <summary>
    /// Базовый репозиторий работы с сущностями в режиме "только чтение"
    /// </summary>
    /// <typeparam name="T">Тип сущности</typeparam>
    public class ReadOnlyEntityRepository<T> : IReadOnlyEntityRepository<T> where T : EntityDao
    {
        protected readonly BaseDbContext Context;

        /// <summary>
        /// Базовый репозиторий работы с сущностями в режиме "только чтение"
        /// </summary>
        /// <param name="context">Контекст работы с БД</param>
        public ReadOnlyEntityRepository(BaseDbContext context)
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
