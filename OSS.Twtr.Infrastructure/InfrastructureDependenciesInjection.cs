using System.Text.Json.Serialization;
using FastEndpoints;
using FastEndpoints.Swagger;
using FluentMigrator.Runner;
using FluentValidation;
using LinqToDB.AspNet;
using LinqToDB.AspNet.Logging;
using LinqToDB.Configuration;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OSS.Twtr.Application;
using OSS.Twtr.Core;
using OSS.Twtr.Management.Application;
using OSS.Twtr.Domain.Repositories;
using OSS.Twtr.Management.Infrastructure.Persistence;
using OSS.Twtr.Management.Infrastructure.Persistence.Migrations;
using OSS.Twtr.Management.Infrastructure.Repositories;

namespace OSS.Twtr.Infrastructure;

public static class InfrastructureDependenciesInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddFastEndpoints();
        services.AddSwaggerDoc(shortSchemaNames: true);
        
        services.AddScoped<IEventDispatcher, EventDispatcher>();
        services.AddMediatR(typeof(ICommand<>).Assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        
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

    public static WebApplication UseInfrastructure(this WebApplication app)
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
        
        app.UseDefaultExceptionHandler();
        app.UseHttpsRedirection();
        
        app.UseFastEndpoints(c =>
        {
            c.Endpoints.ShortNames = true;
            c.Endpoints.RoutePrefix = "api";
            c.Serializer.Options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });
        
        app.UseOpenApi();
        app.UseSwaggerUi3(s =>
        {
            s.ConfigureDefaults();
            s.DocExpansion = "list";
            s.TagsSorter = "alpha";
            s.OperationsSorter = "alpha";
        });

        return app;
    }
}