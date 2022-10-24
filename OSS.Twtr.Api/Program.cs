using OSS.Twtr.Infrastructure;
using OSS.Twtr.Identity.Infrastructure;
using OSS.Twtr.Management.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddManagementFeature(builder.Configuration)
    .AddIdentityFeature(builder.Configuration);

var app = builder.Build();
app.UseInfrastructure()
    .UseManagementFeature()
    .UseIdentityFeature();

app.Run();