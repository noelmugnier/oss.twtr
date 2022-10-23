var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddIdentityManagement(builder.Configuration)
    .AddDataManagement(builder.Configuration);

var app = builder.Build();
app.UseInfrastructure()
    .UseIdentityManagement()
    .UseDataManagement();

app.Run();