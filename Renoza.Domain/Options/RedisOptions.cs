using System.ComponentModel.DataAnnotations;

namespace Renoza.Domain.Options
{
    /// <summary>
    /// Настройки подключения к Redis
    /// </summary>
    public class RedisOptions
    {
        /// <summary>
        /// Строка подключения к Redis
        /// </summary>
        [Required]
        public string ConnectionString { get; set; } = "localhost:6379";

        /// <summary>
        /// Префикс для всех ключей в Redis
        /// </summary>
        public string KeyPrefix { get; set; } = "renoza:";

        /// <summary>
        /// Timeout подключения в миллисекундах
        /// </summary>
        [Range(1000, 60000)]
        public int ConnectTimeout { get; set; } = 5000;

        /// <summary>
        /// Timeout операции синхронизации в миллисекундах
        /// </summary>
        [Range(1000, 60000)]
        public int SyncTimeout { get; set; } = 5000;
    }
}
