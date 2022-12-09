using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OSS.Twtr.Auth.Domain.Entities;

namespace OSS.Twtr.Auth.Infrastructure;

public sealed class AuthDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<ApplicationUser>(b =>
        {            
            b.ToTable("Users");
        });
        
        modelBuilder.Entity<ApplicationRole>(b =>
        {            
            b.ToTable("Roles");
        });
        
        modelBuilder.Entity<IdentityRoleClaim<Guid>>(b =>
        {            
            b.ToTable("RoleClaims");
        });
        
        modelBuilder.Entity<IdentityUserRole<Guid>>(b =>
        {            
            b.ToTable("UserRoles");
        });
        
        modelBuilder.Entity<IdentityUserClaim<Guid>>(b =>
        {            
            b.ToTable("UserClaims");
        });
        
        modelBuilder.Entity<IdentityUserLogin<Guid>>(b =>
        {            
            b.ToTable("UserLogins");
        });
        
        modelBuilder.Entity<IdentityUserToken<Guid>>(b =>
        {            
            b.ToTable("UserTokens");
        });
    }
}

