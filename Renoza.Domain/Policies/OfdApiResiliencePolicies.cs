using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;
using Renoza.Domain.Options;
using System.Net;

namespace Renoza.Domain.Policies
{
    /// <summary>
    /// Политики устойчивости для OFD API с использованием Polly v8
    /// </summary>
    public static class OfdApiResiliencePolicies
    {
        /// <summary>
        /// Создать ResiliencePipeline с комбинацией всех политик
        /// Порядок выполнения (снаружи внутрь):
        /// 1. Retry (повторные попытки)
        /// 2. Circuit Breaker (размыкание при сбоях)
        /// 3. Timeout (таймаут запроса)
        /// </summary>
        public static ResiliencePipeline<HttpResponseMessage> CreateResiliencePipeline(
            OfdApiResilienceOptions options,
            ILogger logger)
        {
            return new ResiliencePipelineBuilder<HttpResponseMessage>()
                // 1. Timeout - внутренний слой (выполняется первым)
                .AddTimeout(new TimeoutStrategyOptions
                {
                    Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds),
                    OnTimeout = args =>
                    {
                        logger.LogWarning(
                            "⏱️ OFD API Timeout после {Timeout}s",
                            args.Timeout.TotalSeconds);
                        return default;
                    }
                })
                // 2. Circuit Breaker - средний слой
                .AddCircuitBreaker(new CircuitBreakerStrategyOptions<HttpResponseMessage>
                {
                    FailureRatio = 1.0, // 100% ошибок (т.е. все запросы должны быть ошибочными)
                    MinimumThroughput = options.CircuitBreakerFailureThreshold,
                    SamplingDuration = TimeSpan.FromSeconds(30),
                    BreakDuration = TimeSpan.FromSeconds(options.CircuitBreakerDurationSeconds),
                    ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                        .Handle<HttpRequestException>()
                        .Handle<TimeoutException>()
                        .HandleResult(response =>
                            response.StatusCode >= HttpStatusCode.InternalServerError || // 5xx
                            response.StatusCode == HttpStatusCode.RequestTimeout), // 408
                    OnOpened = args =>
                    {
                        logger.LogError(
                            "⚠️ OFD API Circuit Breaker ОТКРЫТ на {Duration}s. " +
                            "Превышен порог ошибок.",
                            options.CircuitBreakerDurationSeconds);
                        return default;
                    },
                    OnClosed = args =>
                    {
                        logger.LogInformation("✅ OFD API Circuit Breaker ЗАКРЫТ. Сервис восстановлен.");
                        return default;
                    },
                    OnHalfOpened = args =>
                    {
                        logger.LogInformation("🔄 OFD API Circuit Breaker в состоянии HALF-OPEN. Тестируем восстановление...");
                        return default;
                    }
                })
                // 3. Retry - внешний слой (выполняется последним)
                .AddRetry(new RetryStrategyOptions<HttpResponseMessage>
                {
                    MaxRetryAttempts = options.RetryCount,
                    Delay = TimeSpan.FromSeconds(options.RetryDelaySeconds),
                    BackoffType = DelayBackoffType.Exponential,
                    UseJitter = true, // Добавляем jitter для избежания thundering herd
                    ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                        .Handle<HttpRequestException>()
                        .Handle<TimeoutException>()
                        .HandleResult(response =>
                        {
                            // Retry для временных ошибок (5xx, 408)
                            // НЕ retry для 4xx (клиентские ошибки) и 429 (Rate Limit)
                            return response.StatusCode >= HttpStatusCode.InternalServerError || // 5xx
                                   response.StatusCode == HttpStatusCode.RequestTimeout; // 408
                        }),
                    OnRetry = args =>
                    {
                        var statusCode = args.Outcome.Result?.StatusCode.ToString() ?? "Exception";
                        var errorMessage = args.Outcome.Exception?.Message ??
                                          args.Outcome.Result?.ReasonPhrase ?? "Unknown";

                        logger.LogWarning(
                            "OFD API Retry {RetryCount}/{MaxRetries} после {Delay}s. " +
                            "StatusCode: {StatusCode}, Ошибка: {Error}",
                            args.AttemptNumber,
                            options.RetryCount,
                            args.RetryDelay.TotalSeconds,
                            statusCode,
                            errorMessage);

                        return default;
                    }
                })
                .Build();
        }
    }
}
