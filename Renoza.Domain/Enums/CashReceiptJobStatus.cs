namespace Renoza.Domain.Enums
{
    /// <summary>
    /// Статус запроса на загрузку чека
    /// </summary>
    public enum CashReceiptJobStatus
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
        /// Валидация не пройдена (терминальный статус)
        /// </summary>
        ValidationFailed = 3,

        /// <summary>
        /// Добавлен в очередь на обработку
        /// </summary>
        Queued = 4,

        /// <summary>
        /// Обрабатывается (распознавание)
        /// </summary>
        Processing = 5,

        /// <summary>
        /// Не удалось распознать чек (терминальный статус)
        /// </summary>
        RecognitionFailed = 6,

        /// <summary>
        /// Чек успешно распознан
        /// </summary>
        Recognized = 7,

        /// <summary>
        /// Сохранение данных чека в БД
        /// </summary>
        Saving = 8,

        /// <summary>
        /// Ошибка сохранения в БД (терминальный статус)
        /// </summary>
        SaveFailed = 9,

        /// <summary>
        /// Чек успешно загружен (терминальный статус)
        /// </summary>
        Completed = 10,

        /// <summary>
        /// Запрос отменён пользователем (терминальный статус)
        /// </summary>
        Cancelled = 11
    }
}
