using Microsoft.EntityFrameworkCore;
using OSS.Twtr.Core;
using OSS.Twtr.Domain;

namespace OSS.Twtr.Infrastructure;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>(b =>
        {
            b.HasKey(u => u.Id);
            b.Property(u => u.Id)
                .HasConversion(id => id.Value, guid => UserId.From(guid));
            
            b.Property(t => t.UserName).IsRequired();
            b.Property(t => t.DisplayName);
            b.Property(t => t.Email);
            b.Property(t => t.MemberSince).IsRequired();
            
            b.ToTable("Users");
        });
        
        modelBuilder.Entity<Tweet>(b =>
        {
            b.HasKey(u => u.Id);
            b.Property(u => u.Id)
                .HasConversion(id => id.Value, guid => TweetId.From(guid));

            b.Property(t => t.Message).IsRequired();
            b.Property(t => t.PostedOn).IsRequired();
            
            b.HasOne(t => t.PostedBy)
                .WithMany()
                .HasForeignKey("PostedById")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
            
            b.Navigation(t => t.PostedBy).AutoInclude();
            
            b.ToTable("Tweets");
        });
    }
}