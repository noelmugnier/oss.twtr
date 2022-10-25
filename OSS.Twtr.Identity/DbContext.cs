using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace OSS.Twtr.Identity;

public class DbContext : IdentityDbContext<IdentityUser>
{
    public DbContext(DbContextOptions<DbContext> options): base(options)
    {
    }
}

public class JwtSettings
{
    public const string Section = "Jwt";
    public string Key { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
}