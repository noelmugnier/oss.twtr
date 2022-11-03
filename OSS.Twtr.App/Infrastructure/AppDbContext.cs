using Microsoft.EntityFrameworkCore;
using OSS.Twtr.App.Domain.ValueObjects;

namespace OSS.Twtr.App.Infrastructure;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<UserData>(b =>
        {
            b.HasKey(u => u.Id);
            b.Property(u => u.Id)
                .HasConversion(id => id.Value, guid => UserId.From(guid));
            
            b.Property(t => t.UserName).IsRequired();
            b.Property(t => t.DisplayName);
            b.Property(t => t.Email);
            b.Property(t => t.MemberSince).IsRequired();
            
            b.ToTable(UserData.TableName);
        });
        
        modelBuilder.Entity<TweetData>(b =>
        {
            b.HasKey(u => u.Id);
            b.Property(u => u.Id)
                .HasConversion(id => id.Value, guid => TweetId.From(guid));

            b.Property(t => t.Message).IsRequired();
            b.Property(t => t.PostedOn).IsRequired();
            b.Property(t => t.AuthorId).HasColumnName("PostedById").IsRequired();
            
            b.HasOne<UserData>()
                .WithMany()
                .HasForeignKey(c => c.AuthorId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
            
            b.HasMany<TweetData>()
                .WithOne()
                .HasForeignKey(c => c.ReplyToTweetId)
                .OnDelete(DeleteBehavior.Restrict);
            
            b.ToTable(TweetData.TableName);
        });
    }
}

internal class UserData
{
    public const string TableName = "Users"; 
    public UserId Id { get; set; }
    public string UserName { get; set; }
    public string? DisplayName { get; set; }
    public string? Email { get; set; }
    public DateTime MemberSince { get; set; }
}

internal class TweetData 
{
    public const string TableName = "Tweets"; 
    public TweetId Id { get; set; }
    public string Message { get; set; }
    public DateTime PostedOn { get; set; }
    public UserId AuthorId { get; set; }
    public TweetId? ReplyToTweetId { get; set; }
}
