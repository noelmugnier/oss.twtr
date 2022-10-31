using System.Reflection;
using System.Text.Json.Serialization;
using FastEndpoints;
using FastEndpoints.Swagger;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OSS.Twtr.Common;
using OSS.Twtr.Common.Application;
using OSS.Twtr.Common.Domain;
using OSS.Twtr.Common.Infrastructure;

namespace OSS.Twtr;

public static class DependenciesInjection
{
    public static IServiceCollection AppApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAppFeatures(configuration, new [] {typeof(ICommand<>).Assembly});

        services.AddFastEndpoints(o => o.IncludeAbstractValidators = true);
        services.AddSwaggerDoc(shortSchemaNames: true);
        
        services.AddScoped<IEventDispatcher, EventDispatcher>();
        services.AddScoped<ICommandDispatcher, CommandDispatcher>();
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();
        
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        return services;
    }

    public static WebApplication UseApplication(this WebApplication app)
    {
        app.UseDefaultExceptionHandler();
        app.UseHttpsRedirection();

        app.UseAppFeatures(app.Configuration, new []{typeof(ICommand<>).Assembly});

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

    private static IServiceCollection AddAppFeatures(this IServiceCollection services, IConfiguration configuration, Assembly[] assemblies)
    {
        services.AddMediatR(assemblies);
        
        var configureFeatureType = typeof(IConfigureFeature);
        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes().Where(t => configureFeatureType.IsAssignableFrom(t) && t.IsClass);
            foreach (var featureType in types)
            {
                var feature = (IConfigureFeature) Activator.CreateInstance(featureType);
                feature.Configure(services, configuration);
            }

            services.AddValidatorsFromAssembly(assembly);
        }

        return services;
    }

    private static IApplicationBuilder UseAppFeatures(this IApplicationBuilder app, IConfiguration configuration, Assembly[] assemblies)
    {
        var useFeatureType = typeof(IUseFeature);
        foreach (var assembly in assemblies)
        {
            var types = useFeatureType.Assembly.GetTypes().Where(t => useFeatureType.IsAssignableFrom(t) && t.IsClass);
            foreach (var featureType in types)
            {
                var feature = (IUseFeature) Activator.CreateInstance(featureType);
                feature.Use(app, configuration);
            }
        }

        return app;
    }
}