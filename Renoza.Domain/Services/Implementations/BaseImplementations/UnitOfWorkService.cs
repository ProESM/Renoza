using Renoza.Domain.Services.Interfaces.BaseInterfaces;
using Renoza.Infrastructure.Contexts;

namespace Renoza.Domain.Services.Implementations.BaseImplementations
{
    /// <summary>
    /// Реализация Unit of Work для управления транзакциями
    /// </summary>
    public sealed class UnitOfWorkService : BaseService<RenozaContext>, IUnitOfWorkService
    {
        /// <summary>
        /// Реализация Unit of Work для управления транзакциями
        /// </summary>
        /// <param name="context">Контекст БД</param>
        public UnitOfWorkService(RenozaContext context) : base(context)
        {
        }
    }
}
