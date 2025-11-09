using Renoza.Domain.Services.Interfaces.BaseInterfaces;
using Renoza.Infrastructure.Contexts;

namespace Renoza.Domain.Services.Implementations.BaseImplementations
{
    /// <summary>
    /// Реализация Unit of Work для управления транзакциями
    /// </summary>
    public sealed class UnitOfWorkService : BaseService, IUnitOfWorkService
    {
        /// <summary>
        /// Конструктор Unit of Work
        /// </summary>
        /// <param name="context">Контекст БД</param>
        public UnitOfWorkService(BaseDbContext context) : base(context)
        {
        }
    }
}
