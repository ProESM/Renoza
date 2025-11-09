namespace Renoza.Domain.Services.Interfaces.BaseInterfaces
{
    /// <summary>
    /// Интерфейс базового UnitOfWork-сервиса
    /// </summary>
    public interface IBaseService : IDisposable
    {
        /// <summary>
        /// Сохранить все изменения в БД
        /// </summary>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Количество затронутых записей</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Начать транзакцию
        /// </summary>
        /// <param name="cancellationToken">Токен отмены</param>
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Зафиксировать транзакцию
        /// </summary>
        /// <param name="cancellationToken">Токен отмены</param>
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Откатить транзакцию
        /// </summary>
        /// <param name="cancellationToken">Токен отмены</param>
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }
}
