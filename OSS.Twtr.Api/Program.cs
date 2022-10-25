using OSS.Twtr.Identity;
using OSS.Twtr.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddIdentityFeature(builder.Configuration);

var app = builder.Build();

app.UseIdentityFeature()
    .UseInfrastructure();

app.Run();