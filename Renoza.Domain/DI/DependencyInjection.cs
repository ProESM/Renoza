using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

namespace Renoza.Domain.DI
{
    /// <summary>
    /// Регистрация зависимостей Domain слоя
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Добавить зависимости Domain слоя в DI контейнер
        /// </summary>
        /// <param name="services">Коллекция сервисов</param>
        /// <param name="configuration">Конфигурация приложения</param>
        /// <returns>Коллекция сервисов</returns>
        public static IServiceCollection AddDomainServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Регистрируем конфигурационные опции как Singleton
            var jwtSettings = configuration.GetRequiredConfigurationSection<JwtSettings>("JwtSettings");
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            services.AddSingleton(jwtSettings);
            var passwordPolicyOptions = configuration.GetRequiredConfigurationSection<PasswordPolicyOptions>("PasswordPolicy");
            services.Configure<PasswordPolicyOptions>(configuration.GetSection("PasswordPolicy"));
            services.AddSingleton(passwordPolicyOptions);

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
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IPhoneCountryCodeService, PhoneCountryCodeService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<ICustomerProfileService, CustomerProfileService>();
            services.AddScoped<IWorkerProfileService, WorkerProfileService>();
            services.AddScoped<IEmailVerificationService, EmailVerificationService>();
            services.AddScoped<IPhoneVerificationService, PhoneVerificationService>();

            // Регистрируем валидаторы
            services.AddSingleton<PasswordValidator>();

            return services;
        }
    }
}
