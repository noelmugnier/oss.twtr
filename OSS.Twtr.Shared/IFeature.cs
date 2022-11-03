using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace OSS.Twtr;

public interface IConfigureFeature
{
    IServiceCollection Configure(IServiceCollection services, IConfiguration configuration);
}
public interface IUseFeature
{
    IApplicationBuilder Use(IApplicationBuilder app, IConfiguration configuration);
}

public interface IFeature : IConfigureFeature, IUseFeature
{
}