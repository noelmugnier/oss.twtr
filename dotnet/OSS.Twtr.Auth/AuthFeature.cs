using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using OSS.Twtr.Auth.Application.Services;
using OSS.Twtr.Auth.Application.Settings;
using OSS.Twtr.Auth.Domain.Entities;
using OSS.Twtr.Auth.Infrastructure;
using OSS.Twtr.Auth.Infrastructure.Services;

namespace OSS.Twtr.Auth;

public class AuthFeature : IFeature
{
    public IServiceCollection Configure(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AuthDbContext>(c => 
            c.UseSqlServer(configuration.GetConnectionString("Data"), 
                options => options.MigrationsAssembly(typeof(AuthFeature).Assembly.FullName)));

        services.AddScoped<IUserManager, ApplicationUserManager>();
        services.AddScoped<ISignInManager, ApplicationSignInManager>();
        
        services.AddIdentity<ApplicationUser, ApplicationRole>()
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

    public IApplicationBuilder Use(IApplicationBuilder app, IConfiguration configuration)
    {
        using var scope = app.ApplicationServices.CreateScope();   
        var authContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        authContext.Database.Migrate();
        
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}