using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OSS.Twtr.Application;
using OSS.Twtr.Domain;
using OSS.Twtr.Infrastructure;

namespace OSS.Twtr;

public class SharedFeature : IConfigureFeature
{
    public IServiceCollection Configure(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IEventDispatcher, EventDispatcher>();
        services.AddScoped<ICommandDispatcher, CommandDispatcher>();
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}