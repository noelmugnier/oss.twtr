using FastEndpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OSS.Twtr.Common;
using OSS.Twtr.Common.Application;
using OSS.Twtr.Tweet.Domain.Repositories;
using OSS.Twtr.Tweet.Infrastructure.Persistence;

namespace OSS.Twtr.Tweet;

public class TweetFeature : IFeature
{
    public IServiceCollection Configure(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(c => 
            c.UseSqlServer(configuration.GetConnectionString("Data"), 
                options => options.MigrationsAssembly("OSS.Twtr.Migrations.App")));
        
        services.AddDbContext<IReadRepository, ReadRepository>(c => 
            c.UseSqlServer(configuration.GetConnectionString("Data")));

        services.AddDbContext<ITweetRepository, TweetRepository>(c =>
            c.UseSqlServer(configuration.GetConnectionString("Data")));

        return services;
    }

    public IApplicationBuilder Use(IApplicationBuilder app, IConfiguration configuration)
    {
        using var scope = app.ApplicationServices.CreateScope();   
        var authContext = scope.Resolve<AppDbContext>();
        authContext.Database.Migrate();

        return app;
    }
}