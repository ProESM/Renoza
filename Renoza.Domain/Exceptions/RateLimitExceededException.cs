using Renoza.Common.Base.Exceptions;

namespace Renoza.Domain.Exceptions
{
    /// <summary>
    /// Исключение при превышении лимита запросов (Rate Limit)
    /// </summary>
    public class RateLimitExceededException : BaseException
    {
        /// <summary>
        /// IP адрес, для которого превышен лимит
        /// </summary>
        public string IpAddress { get; }

        /// <summary>
        /// Текущее количество запросов
        /// </summary>
        public int CurrentCount { get; }

        /// <summary>
        /// Максимальное количество запросов
        /// </summary>
        public int Limit { get; }

        public RateLimitExceededException(string message) : base(message)
        {
        }

        public RateLimitExceededException(string message, Exception inner) : base(message, inner)
        {
        }

        public RateLimitExceededException(string ipAddress, int currentCount, int limit)
            : base($"Rate limit превышен для IP {ipAddress}: {currentCount}/{limit}")
        {
            IpAddress = ipAddress;
            CurrentCount = currentCount;
            Limit = limit;
        }
    }
}
