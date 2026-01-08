using Renoza.DbMigration.Attributes;

namespace Renoza.DbMigration.Enums
{
    /// <summary>
    /// Статус задания на верификацию компании
    /// </summary>
    public enum CompanyVerificationJobStatus : short
    {
        /// <summary>
        /// Ожидает обработки
        /// </summary>
        [CompanyVerificationJobStatusDetails(1, "pending", "Ожидает обработки", "Ожидает обработки", true)]
        Pending = 1,

        /// <summary>
        /// Валидация данных
        /// </summary>
        [CompanyVerificationJobStatusDetails(2, "validating", "Валидация данных", "Валидация данных", true)]
        Validating = 2,

        /// <summary>
        /// Ошибка валидации
        /// </summary>
        [CompanyVerificationJobStatusDetails(3, "validation_failed", "Ошибка валидации", "Ошибка валидации", true)]
        ValidationFailed = 3,

        /// <summary>
        /// В очереди на проверку
        /// </summary>
        [CompanyVerificationJobStatusDetails(4, "queued", "В очереди", "В очереди на проверку", true)]
        Queued = 4,

        /// <summary>
        /// Обработка запроса к внешнему сервису
        /// </summary>
        [CompanyVerificationJobStatusDetails(5, "processing", "Обработка", "Обработка запроса к внешнему сервису", true)]
        Processing = 5,

        /// <summary>
        /// Ошибка при запросе к внешнему сервису
        /// </summary>
        [CompanyVerificationJobStatusDetails(6, "external_service_failed", "Ошибка сервиса", "Ошибка при запросе к внешнему сервису", true)]
        ExternalServiceFailed = 6,

        /// <summary>
        /// Данные получены от внешнего сервиса
        /// </summary>
        [CompanyVerificationJobStatusDetails(7, "data_received", "Данные получены", "Данные получены от внешнего сервиса", true)]
        DataReceived = 7,

        /// <summary>
        /// Компания не найдена
        /// </summary>
        [CompanyVerificationJobStatusDetails(8, "company_not_found", "Компания не найдена", "Компания не найдена", true)]
        CompanyNotFound = 8,

        /// <summary>
        /// Сохранение результатов
        /// </summary>
        [CompanyVerificationJobStatusDetails(9, "saving", "Сохранение результатов", "Сохранение результатов", true)]
        Saving = 9,

        /// <summary>
        /// Ошибка сохранения
        /// </summary>
        [CompanyVerificationJobStatusDetails(10, "save_failed", "Ошибка сохранения", "Ошибка сохранения", true)]
        SaveFailed = 10,

        /// <summary>
        /// Завершено успешно (компания верифицирована)
        /// </summary>
        [CompanyVerificationJobStatusDetails(11, "completed", "Завершено успешно", "Завершено успешно (компания верифицирована)", true)]
        Completed = 11,

        /// <summary>
        /// Отклонено (компания ликвидирована или неактивна)
        /// </summary>
        [CompanyVerificationJobStatusDetails(12, "rejected", "Отклонено", "Компания ликвидирована или неактивна", true)]
        Rejected = 12,

        /// <summary>
        /// Отменено
        /// </summary>
        [CompanyVerificationJobStatusDetails(13, "cancelled", "Отменено", "Отменено", true)]
        Cancelled = 13
    }
}
