using Amazon.S3;
using Amazon.Runtime;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Renoza.Domain.Extensions;
using Renoza.Domain.Mappings;
using Renoza.Domain.Options;
using Renoza.Domain.Services.Implementations.BaseImplementations;
using Renoza.Domain.Services.Implementations.RenozaImplementations;
using Renoza.Domain.Services.Interfaces.BaseInterfaces;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;
using Renoza.Domain.Validators;
using Renoza.Infrastructure.Contexts;
using Renoza.Infrastructure.Entities.Renoza;
using Renoza.Infrastructure.Repositories.Implementations.BaseImplementations;
using Renoza.Infrastructure.Repositories.Interfaces.BaseInterfaces;
using Serilog;
using StackExchange.Redis;

namespace Renoza.Domain.DI
{
    /// <summary>
    /// Регистрация зависимостей Domain слоя
    /// </summary>
    public static class DependencyInjection
    {
        // Базовые зависимости (обязательные для всех)
        public static IServiceCollection AddDomainCore(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Регистрируем RenozaContext как Scoped
            // MassTransit автоматически создаёт scope для каждого Consumer,
            // поэтому каждое сообщение будет обрабатываться с новым экземпляром контекста
            var connectionString = configuration.GetConnectionString("Renoza");
            services.AddScoped(_ =>
            {
                var optionsBuilder = new DbContextOptionsBuilder<RenozaContext>();
                optionsBuilder.UseNpgsql(connectionString);
                return new RenozaContext(optionsBuilder.Options);
            });

            // Регистрируем фабрику для создания RenozaContext
            // Для случаев, когда нужно явно создать новый экземпляр вне scope
            services.AddSingleton<Func<RenozaContext>>(_ =>
            {
                return () =>
                {
                    var optionsBuilder = new DbContextOptionsBuilder<RenozaContext>();
                    optionsBuilder.UseNpgsql(connectionString);
                    return new RenozaContext(optionsBuilder.Options);
                };
            });

            // Регистрируем IUnitOfWorkService как Scoped
            // Каждый Consumer получит свой экземпляр UnitOfWorkService, связанный с Scoped RenozaContext
            services.AddScoped<IUnitOfWorkService>(serviceProvider =>
            {
                var context = serviceProvider.GetRequiredService<RenozaContext>();
                return new UnitOfWorkService(context);
            });

            // Регистрируем репозитории как Scoped
            services.AddScoped<IEntityWithIdRepository<UserDao, Guid>>(serviceProvider =>
            {
                var context = serviceProvider.GetRequiredService<RenozaContext>();
                return new EntityWithIdRepository<UserDao, Guid>(context);
            });
            services.AddScoped<IEntityWithIdRepository<UserPasswordDao, long>>(serviceProvider =>
            {
                var context = serviceProvider.GetRequiredService<RenozaContext>();
                return new EntityWithIdRepository<UserPasswordDao, long>(context);
            });
            services.AddScoped<IEntityWithIdRepository<UserPasswordHistoryDao, long>>(serviceProvider =>
            {
                var context = serviceProvider.GetRequiredService<RenozaContext>();
                return new EntityWithIdRepository<UserPasswordHistoryDao, long>(context);
            });
            services.AddScoped<IReadOnlyEntityWithIdRepository<PhoneCountryCodeDao, int>>(serviceProvider =>
            {
                var context = serviceProvider.GetRequiredService<RenozaContext>();
                return new ReadOnlyEntityWithIdRepository<PhoneCountryCodeDao, int>(context);
            });
            services.AddScoped<IEntityWithIdRepository<RoleDao, Guid>>(serviceProvider =>
            {
                var context = serviceProvider.GetRequiredService<RenozaContext>();
                return new EntityWithIdRepository<RoleDao, Guid>(context);
            });
            services.AddScoped<IEntityRepository<UserRoleDao>>(serviceProvider =>
            {
                var context = serviceProvider.GetRequiredService<RenozaContext>();
                return new EntityRepository<UserRoleDao>(context);
            });
            services.AddScoped<IEntityWithIdRepository<CustomerProfileDao, Guid>>(serviceProvider =>
            {
                var context = serviceProvider.GetRequiredService<RenozaContext>();
                return new EntityWithIdRepository<CustomerProfileDao, Guid>(context);
            });
            services.AddScoped<IEntityWithIdRepository<WorkerProfileDao, Guid>>(serviceProvider =>
            {
                var context = serviceProvider.GetRequiredService<RenozaContext>();
                return new EntityWithIdRepository<WorkerProfileDao, Guid>(context);
            });
            services.AddScoped<IEntityWithIdRepository<EmailVerificationDao, Guid>>(serviceProvider =>
            {
                var context = serviceProvider.GetRequiredService<RenozaContext>();
                return new EntityWithIdRepository<EmailVerificationDao, Guid>(context);
            });
            services.AddScoped<IEntityWithIdRepository<PhoneVerificationDao, Guid>>(serviceProvider =>
            {
                var context = serviceProvider.GetRequiredService<RenozaContext>();
                return new EntityWithIdRepository<PhoneVerificationDao, Guid>(context);
            });

            // Регистрируем AutoMapper
            // Сканирует сборку Domain для поиска профилей маппинга (например, UserMapperProfile)
            services.AddAutoMapper(config =>
            {
                config.AddMaps(typeof(UserMapperProfile).Assembly);
            });

            // Регистрируем сервисы
            services.AddScoped<IPhoneCountryCodeService, PhoneCountryCodeService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<ICustomerProfileService, CustomerProfileService>();
            services.AddScoped<IWorkerProfileService, WorkerProfileService>();
            services.AddScoped<IPhoneVerificationService, PhoneVerificationService>();

            return services;
        }

        // Опционально: Jwt и PasswordPolicy
        public static IServiceCollection AddJwtServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            if (configuration.GetSection("JwtSettings").Exists())
            {
                // Регистрируем Jwt настройки
                var jwtSettings = configuration.GetRequiredConfigurationSection<JwtSettings>("JwtSettings");
                services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
                services.AddSingleton(jwtSettings);

                services.AddScoped<IJwtService, JwtService>();
            }
            if (configuration.GetSection("PasswordPolicy").Exists())
            {
                // Регистрируем настройки политики паролей
                var passwordPolicyOptions = configuration.GetRequiredConfigurationSection<PasswordPolicyOptions>("PasswordPolicy");
                services.Configure<PasswordPolicyOptions>(configuration.GetSection("PasswordPolicy"));
                services.AddSingleton(passwordPolicyOptions);

                // Регистрируем валидаторы
                services.AddSingleton<PasswordValidator>();

                services.AddScoped<IUserService, UserService>();
                services.AddScoped<IPasswordService, PasswordService>();
            }
            return services;
        }

        // Опционально: Email сервисы
        public static IServiceCollection AddEmailServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            if (configuration.GetSection("EmailSettings").Exists())
            {
                var emailOptions = configuration.GetRequiredConfigurationSection<EmailOptions>("EmailSettings");
                services.Configure<EmailOptions>(configuration.GetSection("EmailSettings"));
                services.AddSingleton(emailOptions);
                services.AddScoped<IEmailService, EmailService>();
                services.AddScoped<IEmailVerificationService, EmailVerificationService>();
            }
            return services;
        }

        // Опционально: RabbitMQ
        public static IServiceCollection AddRabbitMqServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            if (configuration.GetSection("RabbitMq").Exists())
            {
                var rabbitMqOptions = configuration.GetRequiredConfigurationSection<RabbitMqOptions>("RabbitMq");
                services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMq"));
                services.AddSingleton(rabbitMqOptions);

                if (configuration.GetSection("CashReceiptBrokerOptions").Exists())
                {
                    var cashReceiptBrokerOptions = configuration.GetRequiredConfigurationSection<CashReceiptBrokerOptions>("CashReceiptBrokerOptions");
                    services.Configure<CashReceiptBrokerOptions>(configuration.GetSection("CashReceiptBrokerOptions"));
                    services.AddSingleton(cashReceiptBrokerOptions);
                }
                if (configuration.GetSection("QueueOptions").Exists())
                {
                    var queueOptions = configuration.GetRequiredConfigurationSection<QueueOptions>("QueueOptions");
                    services.Configure<QueueOptions>(configuration.GetSection("QueueOptions"));
                    services.AddSingleton(queueOptions);

                    services.AddScoped<IReceiptService, ReceiptService>();
                }
            }
            return services;
        }

        // Опционально: Redis
        public static IServiceCollection AddRedisServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            if (configuration.GetSection("Redis").Exists())
            {
                // Регистрируем Redis настройки
                var redisOptions = configuration.GetRequiredConfigurationSection<RedisOptions>("Redis");
                services.Configure<RedisOptions>(configuration.GetSection("Redis"));
                services.AddSingleton(redisOptions);

                // Регистрируем Redis ConnectionMultiplexer как Singleton
                services.AddSingleton<IConnectionMultiplexer>(serviceProvider =>
                {
                    var configurationOptions = ConfigurationOptions.Parse(redisOptions.ConnectionString);
                    configurationOptions.ConnectTimeout = redisOptions.ConnectTimeout;
                    configurationOptions.SyncTimeout = redisOptions.SyncTimeout;
                    configurationOptions.AbortOnConnectFail = false; // Не падать при недоступности Redis

                    return ConnectionMultiplexer.Connect(configurationOptions);
                });
            }
            return services;
        }

        // Опционально: Rate Limiting
        public static IServiceCollection AddRateLimitingServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            if (configuration.GetSection("RateLimit").Exists())
            {
                // Регистрируем Rate Limit настройки
                var rateLimitOptions = configuration.GetRequiredConfigurationSection<RateLimitOptions>("RateLimit");
                services.Configure<RateLimitOptions>(configuration.GetSection("RateLimit"));
                services.AddSingleton(rateLimitOptions);

                services.AddSingleton<IRateLimitService, RedisRateLimitService>();
            }
            return services;
        }

        // Опционально: S3
        public static IServiceCollection AddS3Services(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            if (configuration.GetSection("S3Settings").Exists())
            {
                // Регистрируем S3 настройки
                var s3Options = configuration.GetRequiredConfigurationSection<S3Options>("S3Settings");
                services.Configure<S3Options>(configuration.GetSection("S3Settings"));
                services.AddSingleton(s3Options);

                // Регистрируем S3 клиент как Singleton
                services.AddSingleton<IAmazonS3>(serviceProvider =>
                {
                    var config = new AmazonS3Config
                    {
                        ForcePathStyle = s3Options.ForcePathStyle
                    };

                    // Если указан ServiceUrl (для альтернативных S3-совместимых хранилищ)
                    if (!string.IsNullOrEmpty(s3Options.ServiceUrl))
                    {
                        config.ServiceURL = s3Options.ServiceUrl;
                    }
                    // Если указан Region (для AWS S3)
                    else if (!string.IsNullOrEmpty(s3Options.Region))
                    {
                        config.RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(s3Options.Region);
                    }

                    var credentials = new BasicAWSCredentials(s3Options.AccessKey, s3Options.SecretKey);
                    return new AmazonS3Client(credentials, config);
                });

                services.AddScoped<IS3StorageService, S3StorageService>();
            }
            return services;
        }

        // Опционально: Serilog
        public static IHostBuilder ConfigureSerilog(
            this IHostBuilder hostBuilder,
            IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .Enrich.WithThreadId()
                .Enrich.WithProcessId()
                .CreateLogger();

            hostBuilder.UseSerilog();

            return hostBuilder;
        }
    }
}
