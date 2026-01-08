using Renoza.DbMigration.Attributes;

namespace Renoza.DbMigration.Enums
{
    /// <summary>
    /// Статус запроса на загрузку чека
    /// </summary>
    public enum CashReceiptJobStatus
    {
        /// <summary>
        /// Ожидает обработки
        /// </summary>
        [CashReceiptJobStatusDetails(1, "pending", "Ожидает обработки", "Ожидает обработки", true)]
        Pending,

        /// <summary>
        /// Валидация
        /// </summary>
        [CashReceiptJobStatusDetails(2, "validating", "Выполняется валидация", "Выполняется валидация", true)]
        Validating,

        /// <summary>
        /// Ошибка валидации
        /// </summary>
        [CashReceiptJobStatusDetails(3, "validation_failed", "Ошибка валидации", "Валидация завершилась с ошибкой", true)]
        ValidationFailed,

        /// <summary>
        /// В очереди на распознавание
        /// </summary>
        [CashReceiptJobStatusDetails(4, "queued", "В очереди на распознавание", "Задача в очереди на распознавание", true)]
        Queued,

        /// <summary>
        /// Обработка/распознавание
        /// </summary>
        [CashReceiptJobStatusDetails(5, "processing", "Обработка", "Выполняется обработка и распознавание", true)]
        Processing,

        /// <summary>
        /// Ошибка распознавания
        /// </summary>
        [CashReceiptJobStatusDetails(6, "recognition_failed", "Ошибка распознавания", "Распознавание завершилось с ошибкой", true)]
        RecognitionFailed,

        /// <summary>
        /// Успешно распознан
        /// </summary>
        [CashReceiptJobStatusDetails(7, "recognized", "Успешно распознан", "Чек успешно распознан", true)]
        Recognized,

        /// <summary>
        /// Сохранение в БД
        /// </summary>
        [CashReceiptJobStatusDetails(8, "saving", "Сохранение", "Выполняется сохранение в базу данных", true)]
        Saving,

        /// <summary>
        /// Ошибка сохранения
        /// </summary>
        [CashReceiptJobStatusDetails(9, "save_failed", "Ошибка сохранения", "Сохранение завершилось с ошибкой", true)]
        SaveFailed,

        /// <summary>
        /// Завершено успешно
        /// </summary>
        [CashReceiptJobStatusDetails(10, "completed", "Завершено успешно", "Обработка завершена успешно", true)]
        Completed,

        /// <summary>
        /// Отменено
        /// </summary>
        [CashReceiptJobStatusDetails(11, "cancelled", "Отменено", "Задача отменена пользователем", true)]
        Cancelled
    }
}
