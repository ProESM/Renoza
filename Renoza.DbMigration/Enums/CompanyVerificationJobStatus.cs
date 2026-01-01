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
        [CompanyVerificationJobStatusDetails(1, "Pending", true)]
        Pending = 1,

        /// <summary>
        /// Валидация данных
        /// </summary>
        [CompanyVerificationJobStatusDetails(2, "Validating", true)]
        Validating = 2,

        /// <summary>
        /// Ошибка валидации
        /// </summary>
        [CompanyVerificationJobStatusDetails(3, "ValidationFailed", true)]
        ValidationFailed = 3,

        /// <summary>
        /// В очереди на проверку
        /// </summary>
        [CompanyVerificationJobStatusDetails(4, "Queued", true)]
        Queued = 4,

        /// <summary>
        /// Обработка запроса к внешнему сервису
        /// </summary>
        [CompanyVerificationJobStatusDetails(5, "Processing", true)]
        Processing = 5,

        /// <summary>
        /// Ошибка при запросе к внешнему сервису
        /// </summary>
        [CompanyVerificationJobStatusDetails(6, "ExternalServiceFailed", true)]
        ExternalServiceFailed = 6,

        /// <summary>
        /// Данные получены от внешнего сервиса
        /// </summary>
        [CompanyVerificationJobStatusDetails(7, "DataReceived", true)]
        DataReceived = 7,

        /// <summary>
        /// Компания не найдена
        /// </summary>
        [CompanyVerificationJobStatusDetails(8, "CompanyNotFound", true)]
        CompanyNotFound = 8,

        /// <summary>
        /// Сохранение результатов
        /// </summary>
        [CompanyVerificationJobStatusDetails(9, "Saving", true)]
        Saving = 9,

        /// <summary>
        /// Ошибка сохранения
        /// </summary>
        [CompanyVerificationJobStatusDetails(10, "SaveFailed", true)]
        SaveFailed = 10,

        /// <summary>
        /// Завершено успешно (компания верифицирована)
        /// </summary>
        [CompanyVerificationJobStatusDetails(11, "Completed", true)]
        Completed = 11,

        /// <summary>
        /// Отклонено (компания ликвидирована или неактивна)
        /// </summary>
        [CompanyVerificationJobStatusDetails(12, "Rejected", true)]
        Rejected = 12,

        /// <summary>
        /// Отменено
        /// </summary>
        [CompanyVerificationJobStatusDetails(13, "Cancelled", true)]
        Cancelled = 13
    }
}
