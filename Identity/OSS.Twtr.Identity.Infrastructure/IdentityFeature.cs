using FluentMigrator.Runner;
using FluentValidation;
using LinqToDB.AspNet;
using LinqToDB.AspNet.Logging;
using LinqToDB.Configuration;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OSS.Twtr.Identity.Application;
using OSS.Twtr.Identity.Domain.Repositories;
using OSS.Twtr.Identity.Domain.Services;
using OSS.Twtr.Identity.Infrastructure.Persistence;
using OSS.Twtr.Identity.Infrastructure.Persistence.Migrations;
using OSS.Twtr.Identity.Infrastructure.Repositories;
using OSS.Twtr.Identity.Infrastructure.Services;
using OSS.Twtr.Infrastructure;

namespace OSS.Twtr.Identity.Infrastructure;

public static class IdentityFeature
{
    public static IServiceCollection AddIdentityFeature(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(typeof(IIdentityDbContext).Assembly);
        
        services.AddScoped<IIdentityDbContext, IdentityDbContext>();
        services.AddScoped<UnitOfWork<IdentityDbConnection>, IdentityDbContext>();
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IPasswordHasher<object>, PasswordHasher<object>>();
        services.AddScoped<IHasher, Hasher>();
        
        services.AddLinqToDBContext<IdentityDbConnection>((provider, options) => {
            options
                .UseSQLite(configuration.GetConnectionString("Identity"))
                .UseDefaultLogging(provider);
        });
        
        services.AddValidatorsFromAssembly(typeof(IIdentityDbContext).Assembly);
        
        return services;
    }

    public static WebApplication UseIdentityFeature(this WebApplication app)
    {
        var serviceProvider = new ServiceCollection()
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddSQLite()
                .WithGlobalConnectionString(app.Configuration.GetConnectionString("Identity"))
                .ScanIn(typeof(CreateDatabase).Assembly).For.Migrations())
            .AddLogging(lb => lb.AddFluentMigratorConsole())
            .BuildServiceProvider(false);

        using var scope = serviceProvider.CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
        
        return app;
    }
}