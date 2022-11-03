using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OSS.Twtr.App.Domain.Repositories;
using OSS.Twtr.App.Infrastructure;
using OSS.Twtr.Application;

namespace OSS.Twtr.App;

public class TweetFeature : IFeature
{
    public IServiceCollection Configure(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(c => 
            c.UseSqlServer(configuration.GetConnectionString("Data"), 
                options => options.MigrationsAssembly(typeof(TweetFeature).Assembly.FullName)));
        
        services.AddDbContext<IReadRepository, ReadRepository>(c => 
            c.UseSqlServer(configuration.GetConnectionString("Data")));

        services.AddDbContext<ITweetRepository, TweetRepository>(c =>
            c.UseSqlServer(configuration.GetConnectionString("Data")));

        return services;
    }

    public IApplicationBuilder Use(IApplicationBuilder app, IConfiguration configuration)
    {
        using var scope = app.ApplicationServices.CreateScope();   
        var authContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        authContext.Database.Migrate();

        return app;
    }
}