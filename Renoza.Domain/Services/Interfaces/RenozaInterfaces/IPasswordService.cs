using Renoza.Domain.Services.Interfaces.BaseInterfaces;
using Renoza.Infrastructure.Contexts;

namespace Renoza.Domain.Services.Interfaces.RenozaInterfaces
{
    /// <summary>
    /// Интерфейс сервиса работы с паролями пользователей
    /// </summary>
    public interface IPasswordService : IBaseService<RenozaContext>
    {
        Task<bool> SetPasswordAsync(Guid userId, string password, CancellationToken cancellationToken = default);
        Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default);
        Task<bool> ValidatePasswordAsync(Guid userId, string password, CancellationToken cancellationToken = default);
        Task<bool> IsPasswordInHistoryAsync(Guid userId, string newPasswordHash, CancellationToken cancellationToken = default);
        Task<bool> IsPasswordExpiredAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
