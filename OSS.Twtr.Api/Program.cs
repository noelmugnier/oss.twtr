using OSS.Twtr.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(o =>
{
    o.Limits.MaxConcurrentConnections = Int64.MaxValue;
});

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build().UseInfrastructure();
app.Run();