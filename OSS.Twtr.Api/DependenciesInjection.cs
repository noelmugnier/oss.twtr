using System.Reflection;
using FluentValidation;
using MediatR;

namespace OSS.Twtr.Api;

public static class DependenciesInjection
{
    public static IServiceCollection AddFeatures(this IServiceCollection services, IConfiguration configuration, Assembly[] assemblies)
    {
        services.AddAppFeatures(configuration, assemblies);        
        return services;
    }

    public static WebApplication UseFeatures(this WebApplication app, Assembly[] assemblies)
    {
        app.UseAppFeatures(app.Configuration, assemblies);
        return app;
    }

    private static IServiceCollection AddAppFeatures(this IServiceCollection services, IConfiguration configuration, Assembly[] assemblies)
    {
        services.AddMediatR(assemblies);
        
        var baseFeatureType = typeof(IFeature);
        var configureFeatureType = typeof(IConfigureFeature);
        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes().Where(t => (baseFeatureType.IsAssignableFrom(t) || configureFeatureType.IsAssignableFrom(t)) && t.IsClass);
            foreach (var featureType in types)
            {
                if (!configureFeatureType.IsAssignableFrom(featureType))
                    continue;
                
                var feature = (IConfigureFeature) Activator.CreateInstance(featureType);
                feature.Configure(services, configuration);
            }

            services.AddValidatorsFromAssembly(assembly);
        }

        return services;
    }

    private static IApplicationBuilder UseAppFeatures(this IApplicationBuilder app, IConfiguration configuration, Assembly[] assemblies)
    {
        var baseFeatureType = typeof(IFeature);
        var useFeatureType = typeof(IUseFeature);
        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes().Where(t => (baseFeatureType.IsAssignableFrom(t) || useFeatureType
                .IsAssignableFrom(t)) && t.IsClass);
            foreach (var featureType in types)
            {
                if (!useFeatureType.IsAssignableFrom(featureType))
                    continue;

                var feature = (IUseFeature) Activator.CreateInstance(featureType);
                feature.Use(app, configuration);
            }
        }

        return app;
    }
}