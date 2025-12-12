using MassTransit;
using Microsoft.Extensions.Logging;
using Renoza.Domain.Options;

namespace Renoza.Domain.Extensions
{
    /// <summary>
    /// Extension методы для настройки Retry политик MassTransit
    /// </summary>
    public static class MassTransitRetryExtensions
    {
        /// <summary>
        /// Применить retry настройки к конфигуратору consumer
        /// </summary>
        public static void ApplyRetryPolicy<TConsumer>(
            this IConsumerConfigurator<TConsumer> configurator,
            ConsumerRetrySettings settings,
            ILogger logger,
            string consumerName)
            where TConsumer : class
        {
            configurator.UseMessageRetry(retry =>
            {
                if (settings.UseExponentialBackoff)
                {
                    // Экспоненциальный backoff для внешних API
                    retry.Exponential(
                        settings.RetryLimit,
                        TimeSpan.FromSeconds(settings.InitialIntervalSeconds),
                        TimeSpan.FromMinutes(1), // Max delay
                        TimeSpan.FromMilliseconds(100)); // Jitter
                }
                else
                {
                    // Incremental backoff для БД/простых операций
                    retry.Incremental(
                        settings.RetryLimit,
                        TimeSpan.FromSeconds(settings.InitialIntervalSeconds),
                        TimeSpan.FromSeconds(settings.IntervalIncrementSeconds));
                }

                // Исключаем из retry бизнес-ошибки
                retry.Ignore<ArgumentException>();
                retry.Ignore<InvalidOperationException>();

                // Логирование retry попыток
                retry.Handle<Exception>(ex =>
                {
                    logger.LogWarning(
                        "🔄 {ConsumerName} обрабатывает ошибку для Retry. " +
                        "Ошибка: {ErrorType}: {ErrorMessage}",
                        consumerName,
                        ex.GetType().Name,
                        ex.Message);
                    return true;
                });
            });

            // InMemoryOutbox для идемпотентности
            configurator.UseInMemoryOutbox();
        }
    }
}
