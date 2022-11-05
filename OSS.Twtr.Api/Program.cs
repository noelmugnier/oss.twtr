using System.Text.Json.Serialization;
using FastEndpoints;
using FastEndpoints.Swagger;
using OSS.Twtr;
using OSS.Twtr.Api;
using OSS.Twtr.App;
using OSS.Twtr.Auth;

var assemblies = new[] { typeof(AuthFeature).Assembly, typeof(TweetFeature).Assembly, typeof(SharedFeature).Assembly };
var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(o =>
{
    o.Limits.MaxConcurrentConnections = Int64.MaxValue;
});

builder.Services.AddFeatures(builder.Configuration, assemblies);

builder.Services.AddFastEndpoints(o => o.IncludeAbstractValidators = true);
builder.Services.AddSwaggerDoc(shortSchemaNames: true);

var app = builder.Build();

app.UseDefaultExceptionHandler();
app.UseHttpsRedirection();

app.UseFeatures(assemblies);

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

app.Run();

//TODO: Tweet visibility
//TODO: Un-xxx support
//TODO: Remove tweet
//TODO: parse tweet content (#, @, links)
//TODO: Lists support
//TODO: Edit account
//TODO: account picture