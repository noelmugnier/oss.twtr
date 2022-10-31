using OSS.Twtr;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(o =>
{
    o.Limits.MaxConcurrentConnections = Int64.MaxValue;
});

builder.Services.AppApplication(builder.Configuration);

var app = builder.Build().UseApplication();
app.Run();