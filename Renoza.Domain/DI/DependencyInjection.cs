using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Renoza.Domain.Mappings;
using Renoza.Domain.Services.Implementations.BaseImplementations;
using Renoza.Domain.Services.Implementations.RenozaImplementations;
using Renoza.Domain.Services.Interfaces.BaseInterfaces;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;
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
            //services.Configure<CalculationBrokerOptions>(configuration.GetSection("CalculationBrokerOptions"));
            //services.AddSingleton(sp => sp.GetRequiredService<IOptions<CalculationBrokerOptions>>().Value);

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
            services.AddScoped<IEntityWithIdRepository<UserDao, int>>(serviceProvider =>
            {
                var context = serviceProvider.GetRequiredService<RenozaContext>();
                return new EntityWithIdRepository<UserDao, int>(context);
            });

            // Регистрируем AutoMapper
            // Сканирует сборку Domain для поиска профилей маппинга (например, UserMapperProfile)
            services.AddAutoMapper(config =>
            {
                config.AddMaps(typeof(UserMapperProfile).Assembly);
            });

            // Регистрируем сервисы
            services.AddScoped<IUserService, UserService>();

            return services;
        }
    }
}
