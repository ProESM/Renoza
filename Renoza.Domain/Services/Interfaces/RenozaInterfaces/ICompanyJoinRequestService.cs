using Renoza.Common.Base.Helpers;
using Renoza.Domain.Entities.CompanyMembers;

namespace Renoza.Domain.Services.Interfaces.RenozaInterfaces
{
    /// <summary>
    /// Сервис для управления запросами на вступление в компанию
    /// </summary>
    public interface ICompanyJoinRequestService
    {
        /// <summary>
        /// Создать запрос на вступление в компанию
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <param name="companyProfileId">ID профиля компании</param>
        /// <param name="requestMessage">Сообщение от пользователя</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Созданный запрос на вступление</returns>
        Task<Result<CompanyJoinRequest>> CreateJoinRequestAsync(
            Guid userId,
            Guid companyProfileId,
            string? requestMessage = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Одобрить запрос на вступление в компанию
        /// </summary>
        /// <param name="requestId">ID запроса</param>
        /// <param name="reviewerId">ID пользователя, который одобряет запрос</param>
        /// <param name="responseMessage">Сообщение от проверяющего</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Обновленный запрос</returns>
        Task<Result<CompanyJoinRequest>> ApproveJoinRequestAsync(
            Guid requestId,
            Guid reviewerId,
            string? responseMessage = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Отклонить запрос на вступление в компанию
        /// </summary>
        /// <param name="requestId">ID запроса</param>
        /// <param name="reviewerId">ID пользователя, который отклоняет запрос</param>
        /// <param name="responseMessage">Сообщение от проверяющего</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Обновленный запрос</returns>
        Task<Result<CompanyJoinRequest>> RejectJoinRequestAsync(
            Guid requestId,
            Guid reviewerId,
            string? responseMessage = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Отменить запрос на вступление (пользователем)
        /// </summary>
        /// <param name="requestId">ID запроса</param>
        /// <param name="userId">ID пользователя, который отменяет запрос</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Обновленный запрос</returns>
        Task<Result<CompanyJoinRequest>> CancelJoinRequestAsync(
            Guid requestId,
            Guid userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить запрос по ID
        /// </summary>
        /// <param name="requestId">ID запроса</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Запрос на вступление</returns>
        Task<Result<CompanyJoinRequest>> GetJoinRequestAsync(
            Guid requestId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить все запросы пользователя
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список запросов пользователя</returns>
        Task<Result<List<CompanyJoinRequest>>> GetUserJoinRequestsAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить все запросы для компании
        /// </summary>
        /// <param name="companyProfileId">ID профиля компании</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список запросов для компании</returns>
        Task<Result<List<CompanyJoinRequest>>> GetCompanyJoinRequestsAsync(
            Guid companyProfileId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить ожидающие рассмотрения запросы для компании
        /// </summary>
        /// <param name="companyProfileId">ID профиля компании</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список ожидающих запросов</returns>
        Task<Result<List<CompanyJoinRequest>>> GetPendingJoinRequestsAsync(
            Guid companyProfileId,
            CancellationToken cancellationToken = default);
    }
}
