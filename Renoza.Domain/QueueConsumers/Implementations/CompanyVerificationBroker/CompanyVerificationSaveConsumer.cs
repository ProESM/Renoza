using MassTransit;
using Microsoft.Extensions.Logging;
using Renoza.Domain.Entities.CompanyProfiles;
using Renoza.Domain.Enums;
using Renoza.Domain.Messages.CompanyVerification;
using Renoza.Domain.QueueConsumers.Interfaces;
using Renoza.Domain.Services.Interfaces.BaseInterfaces;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;
using Renoza.Infrastructure.Entities.Renoza;
using Renoza.Infrastructure.Repositories.Interfaces.BaseInterfaces;

namespace Renoza.Domain.QueueConsumers.Implementations.CompanyVerificationBroker
{
    /// <summary>
    /// Consumer для сохранения результатов верификации компании в БД
    /// </summary>
    public class CompanyVerificationSaveConsumer : IQueueConsumer<CompanyVerificationSaveMessage>
    {
        /// <summary>
        /// Логгер для записи информации о процессе сохранения результатов верификации
        /// </summary>
        private readonly ILogger<CompanyVerificationSaveConsumer> _logger;

        /// <summary>
        /// Сервис для управления заданиями на верификацию компаний
        /// </summary>
        private readonly ICompanyVerificationJobService _companyVerificationJobService;

        /// <summary>
        /// Сервис для работы с профилями компаний
        /// </summary>
        private readonly ICompanyProfileService _companyProfileService;

        /// <summary>
        /// Репозиторий для сохранения данных верификации компаний в БД
        /// </summary>
        private readonly IEntityWithIdRepository<CompanyVerificationDao, Guid> _companyVerificationRepository;

        /// <summary>
        /// Unit of Work для управления транзакциями и сохранения изменений в БД
        /// </summary>
        private readonly IUnitOfWorkService _unitOfWork;

        /// <summary>
        /// Consumer для сохранения результатов верификации компании в БД
        /// </summary>
        /// <param name="logger">Логгер</param>
        /// <param name="companyVerificationJobService">Сервис для управления заданиями на верификацию компаний</param>
        /// <param name="companyProfileService">Сервис для работы с профилями компаний</param>
        /// <param name="companyVerificationRepository">Репозиторий для сохранения данных верификации компаний в БД</param>
        /// <param name="unitOfWork">Unit of Work для управления транзакциями</param>
        public CompanyVerificationSaveConsumer(
            ILogger<CompanyVerificationSaveConsumer> logger,
            ICompanyVerificationJobService companyVerificationJobService,
            ICompanyProfileService companyProfileService,
            IEntityWithIdRepository<CompanyVerificationDao, Guid> companyVerificationRepository,
            IUnitOfWorkService unitOfWork)
        {
            _logger = logger;
            _companyVerificationJobService = companyVerificationJobService;
            _companyProfileService = companyProfileService;
            _companyVerificationRepository = companyVerificationRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Consume(ConsumeContext<CompanyVerificationSaveMessage> context)
        {
            await ProcessMessage(context.Message);
        }

        private async Task ProcessMessage(CompanyVerificationSaveMessage message)
        {
            _logger.LogInformation("Начато сохранение результатов верификации компании. JobId: {JobId}, INN: {Inn}",
                message.JobId, message.Inn);

            try
            {
                // Определяем статус верификации на основе активности компании
                var isVerified = message.IsActive;
                var verificationStatus = isVerified
                    ? CompanyVerificationJobStatus.Completed
                    : CompanyVerificationJobStatus.Rejected;

                // Определяем тип компании на основе данных от DaData
                var companyType = ParseCompanyType(message.CompanyType);

                _logger.LogInformation("Статус компании определен. JobId: {JobId}, IsActive: {IsActive}, CompanyType: {CompanyType}, Status: {Status}",
                    message.JobId, message.IsActive, companyType, verificationStatus);

                // Создаем сущность верификации
                var companyVerificationDao = new CompanyVerificationDao
                {
                    Id = Guid.NewGuid(),
                    CompanyProfileId = message.CompanyProfileId,
                    Inn = message.Inn,
                    Kpp = message.Kpp,
                    Ogrn = message.Ogrn,
                    FullName = message.FullName,
                    ShortName = message.Name,
                    LegalAddress = message.Address,
                    ActualAddress = message.Address,
                    DirectorName = message.DirectorName,
                    RegistrationDate = message.RegistrationDate.HasValue
                        ? DateOnly.FromDateTime(message.RegistrationDate.Value)
                        : null,
                    CompanyStatus = message.IsActive ? "Действующая" : "Ликвидирована",
                    ExternalServiceResponse = message.JsonData,
                    CreatedAt = DateTime.UtcNow
                };

                // Сохраняем верификацию
                await _companyVerificationRepository.CreateAsync(companyVerificationDao, CancellationToken.None);
                await _unitOfWork.SaveChangesAsync(CancellationToken.None);

                _logger.LogInformation("JobId: {JobId}: Данные верификации сохранены. VerificationId: {VerificationId}",
                    message.JobId, companyVerificationDao.Id);

                // Формируем данные для дублирования ключевых полей в профиле компании
                var updateData = new CompanyProfileUpdateData
                {
                    FullName = message.FullName,
                    ShortName = message.Name,
                    Ogrn = message.Ogrn,
                    Kpp = message.Kpp,
                    Address = message.Address,
                    DirectorName = message.DirectorName,
                    RegistrationDate = message.RegistrationDate.HasValue
                        ? DateOnly.FromDateTime(message.RegistrationDate.Value)
                        : null,
                    Status = message.IsActive ? "ACTIVE" : "LIQUIDATED"
                };

                // Обновляем профиль компании
                var updateResult = await _companyProfileService.UpdateVerificationStatusAsync(
                    message.CompanyProfileId,
                    companyVerificationDao.Id,
                    isVerified,
                    companyType,
                    updateData);

                if (!updateResult.IsSuccess)
                {
                    _logger.LogError("JobId: {JobId}: Ошибка при обновлении статуса профиля компании: {Error}",
                        message.JobId, updateResult.ErrorMessage);

                    await _companyVerificationJobService.UpdateJobStatusAsync(
                        message.JobId,
                        (short)CompanyVerificationJobStatus.SaveFailed,
                        $"Ошибка обновления профиля: {updateResult.ErrorMessage}");

                    return;
                }

                _logger.LogInformation("JobId: {JobId}: Статус профиля компании обновлен",
                    message.JobId);

                // Завершаем задание
                await _companyVerificationJobService.CompleteJobAsync(
                    message.JobId,
                    companyVerificationDao.Id,
                    (short)verificationStatus);

                _logger.LogInformation("JobId: {JobId}: Верификация завершена успешно. Статус: {Status}",
                    message.JobId, verificationStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при сохранении результатов верификации. JobId: {JobId}, INN: {Inn}",
                    message.JobId, message.Inn);

                await _companyVerificationJobService.UpdateJobStatusAsync(
                    message.JobId,
                    (short)CompanyVerificationJobStatus.SaveFailed,
                    $"Внутренняя ошибка при сохранении: {ex.Message}");

                throw;
            }
        }

        /// <summary>
        /// Преобразует строковое представление типа компании из DaData в enum CompanyType
        /// </summary>
        /// <param name="companyTypeString">Тип компании (LEGAL или INDIVIDUAL)</param>
        /// <returns>Значение enum CompanyType</returns>
        private short ParseCompanyType(string? companyTypeString)
        {
            if (string.IsNullOrWhiteSpace(companyTypeString))
                return (short)CompanyType.Unknown;

            return companyTypeString.ToUpperInvariant() switch
            {
                "LEGAL" => (short)CompanyType.Legal,
                "INDIVIDUAL" => (short)CompanyType.Individual,
                _ => (short)CompanyType.Unknown
            };
        }
    }
}
