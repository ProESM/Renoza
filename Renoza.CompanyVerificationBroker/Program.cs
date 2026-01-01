using Renoza.CompanyVerificationBroker;
using Renoza.CompanyVerificationBroker.Configuration;
using Renoza.Domain.DI;
using Serilog;

public class Program
{
    private static IServiceCollection _serviceCollection;

    public static void Main(string[] args)
    {
        var host = CreateHostBuilder(args);

        // Настройка Serilog
        host.ConfigureSerilog(GetConfiguration(args));

        try
        {
            Log.Information("Запуск приложения Renoza.CompanyVerificationBroker");
            host.Build().Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Приложение неожиданно завершилось");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
#if DEBUG
            .UseEnvironment("Development")
#else
                .UseEnvironment("Production")
#endif
            .ConfigureServices((hostContext, services) =>
            {
                // Настройка конфигурации
                hostContext.Configuration = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

                _serviceCollection = Register(hostContext, services);
            });

    private static IConfiguration GetConfiguration(string[] args)
    {
#if DEBUG
        var environment = "Development";
#else
        var environment = "Production";
#endif

        return new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
    }

    private static IServiceCollection Register(HostBuilderContext context, IServiceCollection services)
    {
        Console.WriteLine(context.HostingEnvironment.EnvironmentName);

        // Регистрируем зависимости Domain слоя (DbContext, UnitOfWork, репозитории, AutoMapper, сервисы, конфигурационные опции)
        services.AddDomainCore(context.Configuration)
            .AddRabbitMqServices(context.Configuration);

        services.AddRabbitMqMassTransit();

        return services
            .AddOptions()
            .AddHostedService<CompanyVerificationBroker>();
    }
}
