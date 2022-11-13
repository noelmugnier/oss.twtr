using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenAI.GPT3;
using OpenAI.GPT3.Extensions;
using OpenAI.GPT3.Managers;
using OSS.Twtr.App.Application;
using OSS.Twtr.App.Infrastructure;
using OSS.Twtr.Infrastructure;

namespace OSS.Twtr.App;

public class TweetFeature : IFeature
{
    public IServiceCollection Configure(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(c => 
            c.UseSqlServer(configuration.GetConnectionString("Data"), 
                options => options.MigrationsAssembly(typeof(TweetFeature).Assembly.FullName)));
        
        services.AddDbContext<ReadDbContext>(c => 
            c.UseSqlServer(configuration.GetConnectionString("Data")));

        services.AddScoped<ITweetTokenizer, TweetTokenizer>();
        services.AddScoped<IContinuationTokenManager, ContinuationTokenManager>();
        services.AddOpenAIService(settings => { settings.ApiKey = configuration.GetSection("OpenAI")["ApiKey"]; });

        return services;
    }

    public IApplicationBuilder Use(IApplicationBuilder app, IConfiguration configuration)
    {
        using var scope = app.ApplicationServices.CreateScope();   
        var authContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        authContext.Database.Migrate();
        
        RecurringJob.AddOrUpdate<MediatorHangfireBridge>(
            nameof(AnalyzeTrendsHandler),
            handler => handler.Execute(nameof(AnalyzeTrendsCommand), new AnalyzeTrendsCommand(DateTime.UtcNow)),
            Cron.Minutely);

        return app;
    }
}