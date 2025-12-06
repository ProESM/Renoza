namespace Renoza.Domain.Entities.RateLimits
{
    /// <summary>
    /// Информация о текущем состоянии Rate Limiting для IP адреса
    /// </summary>
    public class RateLimitInfo
    {
        /// <summary>
        /// Текущее количество запросов в окне
        /// </summary>
        public int CurrentCount { get; set; }

        /// <summary>
        /// Максимальное количество запросов в окне
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        /// Длительность временного окна
        /// </summary>
        public TimeSpan WindowDuration { get; set; }

        /// <summary>
        /// Время сброса окна (для Fixed Window)
        /// </summary>
        public DateTime? WindowResetTime { get; set; }

        /// <summary>
        /// Осталось запросов до достижения лимита
        /// </summary>
        public int Remaining => Math.Max(0, Limit - CurrentCount);

        /// <summary>
        /// Достигнут ли лимит
        /// </summary>
        public bool IsLimitReached => CurrentCount >= Limit;
    }
}
