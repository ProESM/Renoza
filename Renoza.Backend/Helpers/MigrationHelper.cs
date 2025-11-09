using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.VersionTableInfo;
using FluentMigrator.Runner;
using Renoza.DbMigration.MetaDataVersion;
using Renoza.DbMigration.Migration;

namespace Renoza.Backend.Helpers
{
    public class MigrationHelper
    {
        /// <summary>
        /// Configure the dependency injection services
        /// </summary>
        public static ServiceProvider CreateServices(IConfiguration configuration, bool isDevelopment = true)
        {
            return new ServiceCollection()
                // Add common FluentMigrator services
                .AddFluentMigratorCore()
                .AddScoped(typeof(IVersionTableMetaData), typeof(CustomMetaDataVersionTable))
                .ConfigureRunner(rb => rb
                    // Add Postgres support to FluentMigrator
                    .AddPostgres()
                    // Set the connection string
                    .WithGlobalConnectionString(configuration.GetConnectionString("Renoza"))
                    // Define the assembly containing the migrations
                    .ScanIn(typeof(InitDb).Assembly)
                    .For.Migrations()
                    .For.EmbeddedResources()
                )
                // Enable logging to console in the FluentMigrator way
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                .Configure<RunnerOptions>(ro => ro.Profile = isDevelopment ? "Development" : "Production")
                // Build the service provider
                .BuildServiceProvider(false);
        }

        /// <summary>
        /// Update the database
        /// </summary>
        public static void UpdateDatabase(IServiceProvider serviceProvider)
        {
            // Instantiate the runner
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

            // Execute the migrations
            runner.MigrateUp();
        }
    }
}
