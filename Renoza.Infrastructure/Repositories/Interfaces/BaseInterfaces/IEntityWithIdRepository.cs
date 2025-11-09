using Renoza.Infrastructure.Entities.Base;

namespace Renoza.Infrastructure.Repositories.Interfaces.BaseInterfaces
{
    /// <summary>
    /// Интерфейс базового репозитория работы с сущностями с идентификаторами
    /// </summary>
    /// <typeparam name="T">Тип сущности</typeparam>
    /// <typeparam name="TKey">Тип идентификатора сущности</typeparam>
    public interface IEntityWithIdRepository<T, TKey> : IEntityRepository<T> where T : EntityWithIdDao<TKey>
    {
        /// <summary>
        /// Возвращает объект по его идентификатору
        /// </summary>
        /// <param name="id">Идентификатор объекта</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Объект</returns>
        Task<T?> GetByIdAsync(TKey id, CancellationToken cancellationToken);

        /// <summary>
        /// Возвращает объект по его идентификатору (только из БД)
        /// </summary>
        /// <param name="id">Идентификатор объекта</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Объект</returns>
        Task<T?> GetByIdFromDatabaseAsync(TKey id, CancellationToken cancellationToken);
    }
}
