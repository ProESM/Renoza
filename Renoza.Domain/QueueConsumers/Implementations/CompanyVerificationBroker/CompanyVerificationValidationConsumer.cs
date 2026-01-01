using MassTransit;
using Microsoft.Extensions.Logging;
using Renoza.Domain.Enums;
using Renoza.Domain.Messages.CompanyVerification;
using Renoza.Domain.Options;
using Renoza.Domain.QueueConsumers.Interfaces;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;
using System.Text.RegularExpressions;

namespace Renoza.Domain.QueueConsumers.Implementations.CompanyVerificationBroker
{
    /// <summary>
    /// Consumer для валидации задания на верификацию компании
    /// </summary>
    public class CompanyVerificationValidationConsumer : IQueueConsumer<CompanyVerificationValidationMessage>
    {
        /// <summary>
        /// Логгер для записи информации о процессе валидации
        /// </summary>
        private readonly ILogger<CompanyVerificationValidationConsumer> _logger;
        /// <summary>
        /// Шина сообщений MassTransit для отправки сообщений в очередь
        /// </summary>
        private readonly IBus _bus;
        /// <summary>
        /// Настройки брокера верификации компаний (имена очередей, настройки API)
        /// </summary>
        private readonly CompanyVerificationBrokerOptions _companyVerificationBrokerOptions;
        /// <summary>
        /// Сервис для управления заданиями на верификацию компаний
        /// </summary>
        private readonly ICompanyVerificationJobService _companyVerificationJobService;
        /// <summary>
        /// Сервис для работы с профилями компаний
        /// </summary>
        private readonly ICompanyProfileService _companyProfileService;

        /// <summary>
        /// Regex для валидации ИНН (10 или 12 цифр)
        /// </summary>
        private static readonly Regex InnPattern = new(@"^\d{10}$|^\d{12}$", RegexOptions.Compiled);

        /// <summary>
        /// Consumer для валидации задания на верификацию компании
        /// </summary>
        /// <param name="logger">Логгер</param>
        /// <param name="bus">Шина сообщений MassTransit для отправки сообщений в очередь</param>
        /// <param name="companyVerificationBrokerOptions">Настройки брокера верификации компаний</param>
        /// <param name="companyVerificationJobService">Сервис для управления заданиями на верификацию компаний</param>
        /// <param name="companyProfileService">Сервис для работы с профилями компаний</param>
        public CompanyVerificationValidationConsumer(
            ILogger<CompanyVerificationValidationConsumer> logger,
            IBus bus,
            CompanyVerificationBrokerOptions companyVerificationBrokerOptions,
            ICompanyVerificationJobService companyVerificationJobService,
            ICompanyProfileService companyProfileService)
        {
            _logger = logger;
            _bus = bus;
            _companyVerificationBrokerOptions = companyVerificationBrokerOptions;
            _companyVerificationJobService = companyVerificationJobService;
            _companyProfileService = companyProfileService;
        }

        public async Task Consume(ConsumeContext<CompanyVerificationValidationMessage> context)
        {
            await ProcessMessage(context.Message);
        }

        private async Task ProcessMessage(CompanyVerificationValidationMessage message)
        {
            _logger.LogInformation("Начата валидация задания на верификацию. JobId: {JobId}, INN: {Inn}",
                message.JobId, message.Inn);

            try
            {
                await _companyVerificationJobService.UpdateJobStatusAsync(
                    message.JobId,
                    (short)CompanyVerificationJobStatus.Validating,
                    "Начата валидация данных компании");

                // 1. Валидация формата ИНН
                if (!InnPattern.IsMatch(message.Inn))
                {
                    _logger.LogWarning("JobId: {JobId}: ИНН имеет неверный формат: {Inn}", message.JobId, message.Inn);

                    await _companyVerificationJobService.UpdateJobStatusAsync(
                        message.JobId,
                        (short)CompanyVerificationJobStatus.ValidationFailed,
                        $"ИНН имеет неверный формат: {message.Inn}");

                    return;
                }

                // 2. Проверка дубликатов - есть ли уже верифицированная компания с таким ИНН
                var existingCompanyResult = await _companyProfileService.GetCompanyProfileByInnAsync(message.Inn);

                if (existingCompanyResult.IsSuccess && existingCompanyResult.Data != null)
                {
                    var existingCompany = existingCompanyResult.Data;

                    // Проверяем, что найденный профиль - это тот же профиль, который мы верифицируем
                    if (existingCompany.Id != message.CompanyProfileId)
                    {
                        // Найден ДРУГОЙ профиль с тем же ИНН - это ошибка данных (дубликат)
                        _logger.LogError("JobId: {JobId}: Обнаружен дубликат профиля компании! ИНН {Inn} уже используется в профиле {ExistingProfileId}, а текущий профиль {CurrentProfileId}",
                            message.JobId, message.Inn, existingCompany.Id, message.CompanyProfileId);

                        await _companyVerificationJobService.UpdateJobStatusAsync(
                            message.JobId,
                            (short)CompanyVerificationJobStatus.ValidationFailed,
                            $"Дубликат профиля: ИНН {message.Inn} уже используется в другом профиле ({existingCompany.Id})");

                        return;
                    }

                    // Это тот же профиль - проверяем, верифицирован ли он уже
                    if (existingCompany.IsCompanyVerified && existingCompany.CompanyVerificationId.HasValue)
                    {
                        _logger.LogInformation("JobId: {JobId}: Профиль компании с ИНН {Inn} уже верифицирован (VerificationId: {VerificationId}). Переиспользуем существующие данные.",
                            message.JobId, message.Inn, existingCompany.CompanyVerificationId.Value);

                        // Просто завершаем job, ссылаясь на существующую верификацию
                        await _companyVerificationJobService.CompleteJobAsync(
                            message.JobId,
                            existingCompany.CompanyVerificationId.Value,
                            (short)CompanyVerificationJobStatus.Completed);

                        _logger.LogInformation("JobId: {JobId}: Job завершен с переиспользованием существующей верификации", message.JobId);

                        return;
                    }
                }

                // 3. Проверка активных заданий на верификацию для этой компании
                // ВАЖНО: Проверяем наличие других активных заданий КРОМЕ текущего
                var hasOtherActiveJob = await _companyVerificationJobService.HasOtherActiveJobAsync(
                    message.CompanyProfileId,
                    message.JobId);

                if (hasOtherActiveJob)
                {
                    _logger.LogWarning("JobId: {JobId}: Для компании уже существует другое активное задание на верификацию", message.JobId);

                    await _companyVerificationJobService.UpdateJobStatusAsync(
                        message.JobId,
                        (short)CompanyVerificationJobStatus.Cancelled,
                        "Для компании уже существует другое активное задание на верификацию");

                    return;
                }

                // 4. Валидация пройдена - отправляем на обработку
                _logger.LogInformation("JobId: {JobId}: Валидация успешна, отправка на обработку", message.JobId);

                await _companyVerificationJobService.UpdateJobStatusAsync(
                    message.JobId,
                    (short)CompanyVerificationJobStatus.Queued,
                    "Отправка запроса в очередь на обработку");

                var companyVerificationProcessingMessage = new CompanyVerificationProcessingMessage
                {
                    JobId = message.JobId,
                    CompanyProfileId = message.CompanyProfileId,
                    Inn = message.Inn
                };

                var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{_companyVerificationBrokerOptions.CompanyVerificationProcessingConsumerQueueName}"));
                await endpoint.Send(companyVerificationProcessingMessage);

                _logger.LogInformation("JobId: {JobId}: Отправлено на обработку", message.JobId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при валидации задания на верификацию. JobId: {JobId}", message.JobId);

                await _companyVerificationJobService.UpdateJobStatusAsync(
                    message.JobId,
                    (short)CompanyVerificationJobStatus.ValidationFailed,
                    $"Ошибка валидации: {ex.Message}");

                throw;
            }
        }
    }
}
