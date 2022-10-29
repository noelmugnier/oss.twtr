using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace OSS.Twtr.Application;

public sealed class AuthDbContext : IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<IdentityUser<Guid>>(b =>
        {            
            b.ToTable("Users");
        });
        
        modelBuilder.Entity<IdentityRole<Guid>>(b =>
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

