using Microsoft.EntityFrameworkCore;

namespace Renoza.Infrastructure.Contexts
{
    /// <summary>
    /// Базовый EF DbContext
    /// </summary>
    public abstract class BaseDbContext : DbContext
    {
        /// <summary>
        /// Базовый EF DbContext
        /// </summary>
        protected BaseDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
