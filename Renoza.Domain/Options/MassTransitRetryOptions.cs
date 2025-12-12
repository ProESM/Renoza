namespace Renoza.Domain.Options
{
    /// <summary>
    /// Настройки Retry политик для MassTransit Consumers
    /// </summary>
    public class MassTransitRetryOptions
    {
        /// <summary>
        /// Настройки для CashReceiptValidationConsumer (минимальные retry)
        /// </summary>
        public ConsumerRetrySettings Validation { get; set; } = new()
        {
            RetryLimit = 2,
            InitialIntervalSeconds = 1,
            IntervalIncrementSeconds = 1
        };

        /// <summary>
        /// Настройки для CashReceiptRecognitionConsumer (агрессивные retry для внешнего API)
        /// </summary>
        public ConsumerRetrySettings Recognition { get; set; } = new()
        {
            RetryLimit = 5,
            InitialIntervalSeconds = 2,
            IntervalIncrementSeconds = 0, // Для экспоненциального backoff
            UseExponentialBackoff = true
        };

        /// <summary>
        /// Настройки для CashReceiptSaveConsumer (умеренные retry для БД/S3)
        /// </summary>
        public ConsumerRetrySettings Save { get; set; } = new()
        {
            RetryLimit = 3,
            InitialIntervalSeconds = 2,
            IntervalIncrementSeconds = 2
        };

        /// <summary>
        /// Настройки для CashReceiptInputConsumer (минимальные retry)
        /// </summary>
        public ConsumerRetrySettings Input { get; set; } = new()
        {
            RetryLimit = 2,
            InitialIntervalSeconds = 1,
            IntervalIncrementSeconds = 1
        };
    }

    /// <summary>
    /// Настройки retry для отдельного Consumer
    /// </summary>
    public class ConsumerRetrySettings
    {
        /// <summary>
        /// Количество попыток повтора
        /// </summary>
        public int RetryLimit { get; set; }

        /// <summary>
        /// Начальная задержка в секундах
        /// </summary>
        public int InitialIntervalSeconds { get; set; }

        /// <summary>
        /// Инкремент задержки в секундах (для incremental backoff)
        /// </summary>
        public int IntervalIncrementSeconds { get; set; }

        /// <summary>
        /// Использовать экспоненциальный backoff вместо incremental
        /// </summary>
        public bool UseExponentialBackoff { get; set; } = false;
    }
}
