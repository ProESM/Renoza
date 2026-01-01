using Renoza.Common.Helpers;
using Renoza.Domain.Entities.CompanyVerification;

namespace Renoza.Domain.Services.Interfaces.RenozaInterfaces
{
    /// <summary>
    /// Сервис для работы с заданиями на верификацию компаний
    /// </summary>
    public interface ICompanyVerificationJobService
    {
        /// <summary>
        /// Создать задание на верификацию компании
        /// </summary>
        /// <param name="companyProfileId">ID профиля компании</param>
        /// <param name="inn">ИНН компании</param>
        /// <param name="ipAddress">IP адрес, с которого создано задание</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Созданное задание</returns>
        Task<Result<CompanyVerificationJob>> CreateVerificationJobAsync(
            Guid companyProfileId,
            string inn,
            string ipAddress,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить задание по ID
        /// </summary>
        /// <param name="id">ID задания</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Задание на верификацию</returns>
        Task<Result<CompanyVerificationJob>> GetJobByIdAsync(Guid id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Проверить, есть ли активное (незавершённое) задание на верификацию для компании
        /// </summary>
        /// <param name="companyProfileId">ID профиля компании</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>True, если есть активное задание</returns>
        Task<bool> HasActiveJobAsync(Guid companyProfileId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Проверить наличие других активных заданий на верификацию (кроме указанного)
        /// </summary>
        /// <param name="companyProfileId">ID профиля компании</param>
        /// <param name="excludeJobId">ID задания, которое нужно исключить из проверки</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>True, если есть другие активные задания</returns>
        Task<bool> HasOtherActiveJobAsync(Guid companyProfileId, Guid excludeJobId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Обновить статус задания
        /// </summary>
        /// <param name="jobId">ID задания</param>
        /// <param name="statusId">ID нового статуса</param>
        /// <param name="comment">Комментарий к изменению статуса</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат операции</returns>
        Task<Result<CompanyVerificationJob>> UpdateJobStatusAsync(
            Guid jobId,
            short statusId,
            string? comment = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Завершить задание с результатом верификации
        /// </summary>
        /// <param name="jobId">ID задания</param>
        /// <param name="companyVerificationId">ID результата верификации</param>
        /// <param name="statusId">ID финального статуса</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат операции</returns>
        Task<Result<CompanyVerificationJob>> CompleteJobAsync(
            Guid jobId,
            Guid companyVerificationId,
            short statusId,
            CancellationToken cancellationToken = default);
    }
}
