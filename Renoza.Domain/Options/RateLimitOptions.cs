using System.ComponentModel.DataAnnotations;

namespace Renoza.Domain.Options
{
    /// <summary>
    /// Настройки Rate Limiting
    /// </summary>
    public class RateLimitOptions
    {
        /// <summary>
        /// Максимальное количество запросов с одного IP за окно времени
        /// </summary>
        [Range(1, int.MaxValue)]
        public int MaxRequestsPerWindow { get; set; } = 100;

        /// <summary>
        /// Длительность временного окна в секундах
        /// </summary>
        [Range(1, 86400)] // От 1 секунды до 24 часов
        public int WindowDurationSeconds { get; set; } = 60;

        /// <summary>
        /// Включить Rate Limiting
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Белый список IP адресов (не подлежат rate limiting)
        /// </summary>
        public List<string> WhitelistedIPs { get; set; } = new();

        /// <summary>
        /// Использовать Sliding Window алгоритм (если false, то Fixed Window)
        /// </summary>
        public bool UseSlidingWindow { get; set; } = true;
    }
}
