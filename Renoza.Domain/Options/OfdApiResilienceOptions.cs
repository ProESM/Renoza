namespace Renoza.Domain.Options
{
    /// <summary>
    /// Настройки устойчивости (Resilience) для OFD API
    /// </summary>
    public class OfdApiResilienceOptions
    {
        /// <summary>
        /// Количество попыток повтора при временных сбоях
        /// </summary>
        public int RetryCount { get; set; } = 3;

        /// <summary>
        /// Базовая задержка между попытками в секундах (используется для экспоненциального роста)
        /// </summary>
        public int RetryDelaySeconds { get; set; } = 2;

        /// <summary>
        /// Количество последовательных ошибок для размыкания Circuit Breaker
        /// </summary>
        public int CircuitBreakerFailureThreshold { get; set; } = 5;

        /// <summary>
        /// Длительность разрыва цепи в секундах (Open state)
        /// </summary>
        public int CircuitBreakerDurationSeconds { get; set; } = 30;

        /// <summary>
        /// Timeout для HTTP запроса в секундах
        /// </summary>
        public int TimeoutSeconds { get; set; } = 10;
    }
}
