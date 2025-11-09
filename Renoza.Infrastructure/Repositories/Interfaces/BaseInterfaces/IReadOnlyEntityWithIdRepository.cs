using Renoza.Infrastructure.Entities.Base;

namespace Renoza.Infrastructure.Repositories.Interfaces.BaseInterfaces
{
    /// <summary>
    /// Интерфейс базового репозитория работы с сущностями с идентификаторами в режиме "только чтение"
    /// </summary>
    /// <typeparam name="T">Тип сущности</typeparam>
    /// <typeparam name="TKey">Тип идентификатора сущности</typeparam>
    public interface IReadOnlyEntityWithIdRepository<T, TKey> : IReadOnlyEntityRepository<T> where T : EntityWithIdDao<TKey>
    {
        /// <summary>
        /// Возвращает объект по его идентификатору
        /// </summary>
        /// <param name="id">Идентификатор объекта</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Объект</returns>
        Task<T?> GetByIdAsync(TKey id, CancellationToken cancellationToken);
    }
}
