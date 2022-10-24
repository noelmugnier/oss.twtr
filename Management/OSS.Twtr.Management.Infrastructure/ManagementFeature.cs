using FluentMigrator.Runner;
using FluentValidation;
using LinqToDB.AspNet;
using LinqToDB.AspNet.Logging;
using LinqToDB.Configuration;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OSS.Twtr.Infrastructure;
using OSS.Twtr.Management.Application;
using OSS.Twtr.Management.Domain.Repositories;
using OSS.Twtr.Management.Infrastructure.Persistence;
using OSS.Twtr.Management.Infrastructure.Persistence.Migrations;
using OSS.Twtr.Management.Infrastructure.Repositories;

namespace OSS.Twtr.Management.Infrastructure;

public static class ManagementFeature
{
    public static IServiceCollection AddManagementFeature(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(typeof(IDataDbContext).Assembly);
        
        services.AddScoped<IDataDbContext, DataDbContext>();
        services.AddScoped<UnitOfWork<DataDbConnection>, DataDbContext>();
        services.AddScoped<ITweetRepository, TweetRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddLinqToDBContext<DataDbConnection>((provider, options) => {
            options
                .UseSQLite(configuration.GetConnectionString("Data"))
                .UseDefaultLogging(provider);
        });

        services.AddValidatorsFromAssembly(typeof(IDataDbContext).Assembly);
        
        return services;
    }

    public static WebApplication UseManagementFeature(this WebApplication app)
    {
        var serviceProvider = new ServiceCollection()
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddSQLite()
                .WithGlobalConnectionString(app.Configuration.GetConnectionString("Data"))
                .ScanIn(typeof(CreateDatabase).Assembly).For.Migrations())
            .AddLogging(lb => lb.AddFluentMigratorConsole())
            .BuildServiceProvider(false);

        using var scope = serviceProvider.CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
        
        return app;
    }
}