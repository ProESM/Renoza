using Renoza.Common.Base.Repositories;
using Renoza.Infrastructure.Entities.Base;

namespace Renoza.Infrastructure.Repositories.Interfaces.BaseInterfaces
{
    /// <summary>
    /// Интерфейс базового репозитория работы с сущностями в режиме "только чтение"
    /// </summary>
    /// <typeparam name="T">Тип сущности</typeparam>
    public interface IReadOnlyEntityRepository<T> : IRepository where T : EntityDao
    {
        /// <summary>
        /// Возвращает интерфейс для запроса
        /// </summary>
        /// <returns>Интерфейс для запроса</returns>
        IQueryable<T> GetQueryable();
    }
}
