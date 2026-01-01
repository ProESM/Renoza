namespace Renoza.Domain.Enums
{
    /// <summary>
    /// Статус задания на верификацию компании
    /// </summary>
    public enum CompanyVerificationJobStatus : short
    {
        /// <summary>
        /// Ожидает обработки
        /// </summary>
        Pending = 1,

        /// <summary>
        /// Валидация данных
        /// </summary>
        Validating = 2,

        /// <summary>
        /// Ошибка валидации
        /// </summary>
        ValidationFailed = 3,

        /// <summary>
        /// В очереди на проверку
        /// </summary>
        Queued = 4,

        /// <summary>
        /// Обработка запроса к внешнему сервису
        /// </summary>
        Processing = 5,

        /// <summary>
        /// Ошибка при запросе к внешнему сервису
        /// </summary>
        ExternalServiceFailed = 6,

        /// <summary>
        /// Данные получены от внешнего сервиса
        /// </summary>
        DataReceived = 7,

        /// <summary>
        /// Компания не найдена
        /// </summary>
        CompanyNotFound = 8,

        /// <summary>
        /// Сохранение результатов
        /// </summary>
        Saving = 9,

        /// <summary>
        /// Ошибка сохранения
        /// </summary>
        SaveFailed = 10,

        /// <summary>
        /// Завершено успешно (компания верифицирована)
        /// </summary>
        Completed = 11,

        /// <summary>
        /// Отклонено (компания ликвидирована или неактивна)
        /// </summary>
        Rejected = 12,

        /// <summary>
        /// Отменено
        /// </summary>
        Cancelled = 13
    }
}
