using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using OSS.Twtr.TweetManagement.Infrastructure.Migrations;

var serviceProvider = CreateServices();

// Put the database update into a scope to ensure
// that all resources will be disposed.
using (var scope = serviceProvider.CreateScope())
{
    UpdateDatabase(scope.ServiceProvider);
}
    
static IServiceProvider CreateServices()
{
    return new ServiceCollection()
        // Add common FluentMigrator services
        .AddFluentMigratorCore()
        .ConfigureRunner(rb => rb
            // Add SQLite support to FluentMigrator
            .AddSQLite()
            // Set the connection string
            .WithGlobalConnectionString("Data Source=../../../../OSS.Twtr.Api/data.db")
            // Define the assembly containing the migrations
            .ScanIn(typeof(CreateDatabase).Assembly).For.Migrations())
        // Enable logging to console in the FluentMigrator way
        .AddLogging(lb => lb.AddFluentMigratorConsole())
        // Build the service provider
        .BuildServiceProvider(false);
}

static void UpdateDatabase(IServiceProvider serviceProvider)
{
    // Instantiate the runner
    var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

    // Execute the migrations
    runner.MigrateUp();
}