using FluentValidation;
using LinqToDB.AspNet;
using LinqToDB.AspNet.Logging;
using LinqToDB.Configuration;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using OSS.Twtr.Infrastructure;
using OSS.Twtr.TweetManagement.Application;
using OSS.Twtr.TweetManagement.Domain;
using OSS.Twtr.TweetManagement.Infrastructure;

namespace Microsoft.Extensions.DependencyInjection;

public static class DataDependenciesInjection
{
    public static IServiceCollection AddDataManagement(this IServiceCollection services, IConfiguration configuration)
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

    public static WebApplication UseDataManagement(this WebApplication app)
    {
        return app;
    }
}