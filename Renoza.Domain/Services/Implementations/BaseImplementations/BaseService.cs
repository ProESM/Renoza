using Microsoft.EntityFrameworkCore.Storage;
using Renoza.Domain.Services.Interfaces.BaseInterfaces;
using Renoza.Infrastructure.Contexts;

namespace Renoza.Domain.Services.Implementations.BaseImplementations
{
    /// <summary>
    /// Абстрактный базовый UnitOfWork-сервис
    /// </summary>
    public abstract class BaseService<T> : IBaseService<T>, IAsyncDisposable where T : BaseDbContext
    {
        protected readonly T Context;

        /// <summary>
        /// Транзакция БД
        /// </summary>
        private IDbContextTransaction? _transaction;

        /// <summary>
        /// Абстрактный базовый UnitOfWork-сервис
        /// </summary>
        /// <param name="context">Контекст БД</param>
        protected BaseService(T context)
        {
            Context = context;
        }

        /// <summary>
        /// Сохранить все изменения в БД
        /// </summary>
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await Context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Начать транзакцию
        /// </summary>
        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
            {
                throw new InvalidOperationException("Транзакция уже начата");
            }

            _transaction = await Context.Database.BeginTransactionAsync(cancellationToken);
        }

        /// <summary>
        /// Зафиксировать транзакцию
        /// </summary>
        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("Транзакция не начата");
            }

            try
            {
                await _transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                // Пытаемся откатить транзакцию, но не скрываем исходное исключение
                try
                {
                    if (_transaction != null)
                    {
                        await _transaction.RollbackAsync(cancellationToken);
                        await _transaction.DisposeAsync();
                        _transaction = null;
                    }
                }
                catch
                {
                    // Игнорируем ошибки rollback, чтобы не скрыть исходное исключение
                }
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        /// <summary>
        /// Откатить транзакцию
        /// </summary>
        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("Транзакция не начата");
            }

            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        #region IDisposable and IAsyncDisposable

        /// <summary>
        /// Освободить ресурсы
        /// </summary>
        public void Dispose()
        {
            _transaction?.Dispose();
            _transaction = null;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Освободить ресурсы асинхронно
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
