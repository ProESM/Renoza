using MassTransit;
using Renoza.Domain.QueueConsumers.Interfaces;
using System.Reflection;
using Newtonsoft.Json;
using Renoza.Common.Base.Helpers;
using Renoza.Domain.Options;

namespace Renoza.CashReceiptBroker.Configuration
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
                            && t.FullName != null && t.FullName.Contains("CashReceiptBroker"))
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

                    var cashReceiptBrokerOptions = context.GetRequiredService<CashReceiptBrokerOptions>();

                    var cashReceiptBrokerProperties = cashReceiptBrokerOptions.GetPropertiesDictionary();

                    foreach (var consumer in consumers)
                    {
                        var queueName = $"{consumer.Name}";

                        var cashReceiptBrokerPropertyKey = $"{consumer.Name}QueueName";
                        if (cashReceiptBrokerOptions != null && cashReceiptBrokerProperties.ContainsKey(cashReceiptBrokerPropertyKey))
                        {
                            if (cashReceiptBrokerProperties[cashReceiptBrokerPropertyKey] != null)
                            {
                                queueName = (string)cashReceiptBrokerProperties[cashReceiptBrokerPropertyKey]!;
                            }
                        }

                        switch (consumer.Name)
                        {
                            //case nameof(PrepareScenarioForLoadOutConsumer):
                            //    cfg.ReceiveEndpoint(queueName, e =>
                            //    {
                            //        e.ConfigureConsumer(context, consumer);
                            //        e.PrefetchCount = 1;
                            //    });
                            //    break;
                            //case nameof(RecreateScenarioPartitionsConsumer):
                            default:
                                cfg.ReceiveEndpoint(queueName, e =>
                                {
                                    e.ConfigureConsumer(context, consumer);
                                });
                                break;
                        }
                    }

                    //// Объявление очереди
                    //cfg.Publish<IQueueMessage>(x =>
                    //{
                    //    x.ExchangeType = ExchangeType.Fanout; // or Direct based on needs
                    //    x.BindQueue(cashReceiptBrokerOptions.ExternalPromoForecastCalculationJobCompletionQueueName, cashReceiptBrokerOptions.ExternalPromoForecastCalculationJobCompletionQueueName, e =>
                    //    {
                    //        e.RoutingKey = $"{cashReceiptBrokerOptions.ExternalPromoForecastCalculationJobCompletionQueueName}.key";
                    //        e.ExchangeType = ExchangeType.Direct;
                    //    });
                    //});
                });

                foreach (var consumer in consumers)
                {
                    config.AddConsumers(consumer);
                }
            });

            return services;
        }
    }
}
