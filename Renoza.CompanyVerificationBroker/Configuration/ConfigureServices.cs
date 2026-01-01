using MassTransit;
using Microsoft.Extensions.Logging;
using Renoza.Domain.QueueConsumers.Interfaces;
using Renoza.Domain.QueueConsumers.Implementations.CompanyVerificationBroker;
using System.Reflection;
using Newtonsoft.Json;
using Renoza.Common.Base.Helpers;
using Renoza.Domain.Options;
using Renoza.Domain.Extensions;

namespace Renoza.CompanyVerificationBroker.Configuration
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddRabbitMqMassTransit(this IServiceCollection services)
        {
            var consumers = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a =>
                {
                    try
                    {
                        return a.GetTypes();
                    }
                    catch (ReflectionTypeLoadException ex)
                    {
                        // return the ones that could be loaded
                        return ex.Types.Where(t => t != null).Select(t => t!);
                    }
                    catch
                    {
                        return Enumerable.Empty<Type>();
                    }
                })
                .Where(t => t.GetInterfaces().Any(i =>
                                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQueueConsumer<>))
                            && !t.IsAbstract
                            && t.FullName != null && t.FullName.Contains("CompanyVerificationBroker"))
                .ToList();

            services.AddMassTransit(config =>
            {
                config.UsingRabbitMq((context, cfg) =>
                {
                    var rabbitMqOptions = context.GetRequiredService<RabbitMqOptions>();

                    cfg.Host($"rabbitmq://{rabbitMqOptions.Host}:{rabbitMqOptions.Port}/{rabbitMqOptions.VirtualHost}", configurator =>
                    {
                        configurator.Username(rabbitMqOptions.Username);
                        configurator.Password(rabbitMqOptions.Password);
                    });

                    // Configure Newtonsoft.Json as the serializer
                    cfg.UseNewtonsoftJsonSerializer();

                    // Optionally, configure Newtonsoft.Json settings
                    cfg.ConfigureNewtonsoftJsonSerializer(settings =>
                    {
                        settings.NullValueHandling = NullValueHandling.Include;
                        settings.DefaultValueHandling = DefaultValueHandling.Include;
                        // Add any other Newtonsoft.Json settings you require
                        return settings;
                    });

                    var companyVerificationBrokerOptions = context.GetRequiredService<CompanyVerificationBrokerOptions>();
                    var massTransitRetryOptions = context.GetRequiredService<MassTransitRetryOptions>();
                    var loggerFactory = context.GetRequiredService<ILoggerFactory>();
                    var logger = loggerFactory.CreateLogger("MassTransitRetry");

                    var companyVerificationBrokerProperties = companyVerificationBrokerOptions.GetPropertiesDictionary();

                    foreach (var consumer in consumers)
                    {
                        var queueName = $"{consumer.Name}";

                        var companyVerificationBrokerPropertyKey = $"{consumer.Name}QueueName";
                        if (companyVerificationBrokerOptions != null && companyVerificationBrokerProperties.ContainsKey(companyVerificationBrokerPropertyKey))
                        {
                            if (companyVerificationBrokerProperties[companyVerificationBrokerPropertyKey] != null)
                            {
                                queueName = (string)companyVerificationBrokerProperties[companyVerificationBrokerPropertyKey]!;
                            }
                        }

                        // Определяем retry настройки для каждого типа consumer
                        ConsumerRetrySettings? retrySettings = consumer.Name switch
                        {
                            "CompanyVerificationValidationConsumer" => massTransitRetryOptions.Validation,
                            "CompanyVerificationProcessingConsumer" => massTransitRetryOptions.Recognition,
                            "CompanyVerificationSaveConsumer" => massTransitRetryOptions.Save,
                            _ => null
                        };

                        cfg.ReceiveEndpoint(queueName, e =>
                        {
                            e.ConfigureConsumer(context, consumer);

                            // Применяем retry политики, если они определены для этого consumer
                            if (retrySettings != null)
                            {
                                e.UseMessageRetry(retry =>
                                {
                                    if (retrySettings.UseExponentialBackoff)
                                    {
                                        retry.Exponential(
                                            retrySettings.RetryLimit,
                                            TimeSpan.FromSeconds(retrySettings.InitialIntervalSeconds),
                                            TimeSpan.FromMinutes(1),
                                            TimeSpan.FromMilliseconds(100));
                                    }
                                    else
                                    {
                                        retry.Incremental(
                                            retrySettings.RetryLimit,
                                            TimeSpan.FromSeconds(retrySettings.InitialIntervalSeconds),
                                            TimeSpan.FromSeconds(retrySettings.IntervalIncrementSeconds));
                                    }

                                    retry.Ignore<ArgumentException>();
                                    retry.Ignore<InvalidOperationException>();

                                    retry.Handle<Exception>(ex =>
                                    {
                                        logger.LogWarning(
                                            "🔄 {ConsumerName} обрабатывает ошибку для Retry. Ошибка: {ErrorType}: {ErrorMessage}",
                                            consumer.Name,
                                            ex.GetType().Name,
                                            ex.Message);
                                        return true;
                                    });
                                });

                                // InMemoryOutbox для идемпотентности
                                e.UseInMemoryOutbox(context);
                            }
                        });
                    }
                });

                // Регистрируем все consumers (без retry настроек на этом этапе)
                foreach (var consumer in consumers)
                {
                    config.AddConsumers(consumer);
                }
            });

            return services;
        }
    }
}
