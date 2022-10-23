using FluentValidation;
using LinqToDB.AspNet;
using LinqToDB.AspNet.Logging;
using LinqToDB.Configuration;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using OSS.Twtr.AccountManagement.Application;
using OSS.Twtr.AccountManagement.Domain;
using OSS.Twtr.AccountManagement.Infrastructure;
using OSS.Twtr.Domain.Services;
using OSS.Twtr.Infrastructure;
using OSS.Twtr.Infrastructure.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class IdentityDependenciesInjection
{
    public static IServiceCollection AddIdentityManagement(this IServiceCollection services, IConfiguration configuration)
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

    public static WebApplication UseIdentityManagement(this WebApplication app)
    {
        return app;
    }
}