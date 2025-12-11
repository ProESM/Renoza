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
        [CashReceiptJobStatusDetails(1, "Pending", true)]
        Pending,

        /// <summary>
        /// Валидация
        /// </summary>
        [CashReceiptJobStatusDetails(2, "Validating", true)]
        Validating,

        /// <summary>
        /// Ошибка валидации
        /// </summary>
        [CashReceiptJobStatusDetails(3, "ValidationFailed", true)]
        ValidationFailed,

        /// <summary>
        /// В очереди на распознавание
        /// </summary>
        [CashReceiptJobStatusDetails(4, "Queued", true)]
        Queued,

        /// <summary>
        /// Обработка/распознавание
        /// </summary>
        [CashReceiptJobStatusDetails(5, "Processing", true)]
        Processing,

        /// <summary>
        /// Ошибка распознавания
        /// </summary>
        [CashReceiptJobStatusDetails(6, "RecognitionFailed", true)]
        RecognitionFailed,

        /// <summary>
        /// Успешно распознан
        /// </summary>
        [CashReceiptJobStatusDetails(7, "Recognized", true)]
        Recognized,

        /// <summary>
        /// Сохранение в БД
        /// </summary>
        [CashReceiptJobStatusDetails(8, "Saving", true)]
        Saving,

        /// <summary>
        /// Ошибка сохранения
        /// </summary>
        [CashReceiptJobStatusDetails(9, "SaveFailed", true)]
        SaveFailed,

        /// <summary>
        /// Завершено успешно
        /// </summary>
        [CashReceiptJobStatusDetails(10, "Completed", true)]
        Completed,

        /// <summary>
        /// Отменено
        /// </summary>
        [CashReceiptJobStatusDetails(11, "Cancelled", true)]
        Cancelled
    }
}
