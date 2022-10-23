using System.Text.Json.Serialization;
using FastEndpoints;
using FastEndpoints.Swagger;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using OSS.Twtr.Application;
using OSS.Twtr.Domain.Services;
using OSS.Twtr.Infrastructure.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class InfrastructureDependenciesInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddFastEndpoints();
        services.AddSwaggerDoc(shortSchemaNames: true);
        
        services.AddScoped<IEventDispatcher, EventDispatcher>();
        services.AddMediatR(typeof(ICommand<>).Assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }

    public static WebApplication UseInfrastructure(this WebApplication app)
    {
        app.UseDefaultExceptionHandler();
        app.UseHttpsRedirection();
        app.UseAuthorization();
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