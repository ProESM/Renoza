using Renoza.Common.Base.Helpers;
using Renoza.Domain.Entities.CompanyMembers;

namespace Renoza.Domain.Services.Interfaces.RenozaInterfaces
{
    /// <summary>
    /// Сервис для работы с участниками компании
    /// </summary>
    public interface ICompanyMemberService
    {
        /// <summary>
        /// Добавить участника в компанию
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <param name="companyProfileId">ID профиля компании</param>
        /// <param name="memberRoleId">ID роли участника</param>
        /// <param name="position">Должность (опционально)</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Созданный участник компании</returns>
        Task<Result<CompanyMember>> AddMemberAsync(
            Guid userId,
            Guid companyProfileId,
            Guid memberRoleId,
            string? position = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить участника компании по ID пользователя и ID компании
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <param name="companyProfileId">ID профиля компании</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Участник компании</returns>
        Task<Result<CompanyMember>> GetMemberAsync(
            Guid userId,
            Guid companyProfileId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить всех активных участников компании
        /// </summary>
        /// <param name="companyProfileId">ID профиля компании</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список участников компании</returns>
        Task<Result<List<CompanyMember>>> GetCompanyMembersAsync(
            Guid companyProfileId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Обновить роль участника в компании
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <param name="companyProfileId">ID профиля компании</param>
        /// <param name="newMemberRoleId">Новый ID роли участника</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Обновленный участник компании</returns>
        Task<Result<CompanyMember>> UpdateMemberRoleAsync(
            Guid userId,
            Guid companyProfileId,
            Guid newMemberRoleId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Удалить участника из компании (мягкое удаление)
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <param name="companyProfileId">ID профиля компании</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат операции</returns>
        Task<Result<bool>> RemoveMemberAsync(
            Guid userId,
            Guid companyProfileId,
            CancellationToken cancellationToken = default);
    }
}
