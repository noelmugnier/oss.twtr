using System.Text;
using System.Text.Json.Serialization;
using FastEndpoints;
using FastEndpoints.Swagger;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using OSS.Twtr.Application;
using OSS.Twtr.Core;
using OSS.Twtr.Identity.Endpoints;

namespace OSS.Twtr.Infrastructure;

public static class DependenciesInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddFastEndpoints(o => o.IncludeAbstractValidators = true);
        services.AddSwaggerDoc(shortSchemaNames: true);
        
        services.AddScoped<IEventDispatcher, EventDispatcher>();
        services.AddMediatR(typeof(ICommand<>).Assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        
        services.AddDbContext<AppDbContext>(c => c.UseSqlServer(configuration.GetConnectionString("Data"), options => options.MigrationsAssembly("OSS.Twtr.Migrations.App")));
        services.AddDbContext<AuthDbContext>(c => c.UseSqlServer(configuration.GetConnectionString("Data"), options => options.MigrationsAssembly("OSS.Twtr.Migrations.Auth")));
        
        services.AddValidatorsFromAssembly(typeof(LoginCommand).Assembly);
        services.AddIdentity<IdentityUser<Guid>, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<AuthDbContext>()
            .AddDefaultTokenProviders();
        
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.Section));

        services.Configure<IdentityOptions>(options =>
        {
            // Password settings.
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 6;
            options.Password.RequiredUniqueChars = 0;

            // Lockout settings.
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // User settings.
            options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            options.User.RequireUniqueEmail = false;
        });
        
        services.ConfigureApplicationCookie(options =>
        {
            // Cookie settings
            options.Cookie.HttpOnly = true;
            options.ExpireTimeSpan = TimeSpan.FromDays(30);
            options.SlidingExpiration = true;
        });

        services
            .AddAuthentication()
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("auth", new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddAuthenticationSchemes(IdentityConstants.ApplicationScheme)
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .Build());
        });
        
        return services;
    }

    public static WebApplication UseInfrastructure(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        
        var authContext = scope.Resolve<AuthDbContext>();
        authContext.Database.Migrate();
        
        var appContext = scope.Resolve<AppDbContext>();
        appContext.Database.Migrate();
       
        app.UseDefaultExceptionHandler();
        app.UseHttpsRedirection();
        
        app.UseAuthentication();
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