using Microsoft.EntityFrameworkCore;
using OSS.Twtr.Common.Application;

namespace OSS.Twtr.Tweet.Infrastructure.Persistence;

internal sealed class ReadRepository : DbContext, IReadRepository
{
    public ReadRepository(DbContextOptions<ReadRepository> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<ReadOnlyAuthor>(b =>
        {
            b.HasKey(u => u.Id);

            b.Property(c => c.MemberSince);
            b.Property(c => c.UserName);
            b.Property(c => c.DisplayName);
            b.Property(c => c.Email);
            
            b.ToTable(UserData.TableName);
        });

        modelBuilder.Entity<ReadOnlyTweet>(b =>
        {
            b.HasKey(u => u.Id);
            
            b.Property(c => c.AuthorId).HasColumnName("PostedById");
            b.Property(c => c.Message);
            b.Property(c => c.PostedOn);

            b.HasOne(c => c.Author)
                .WithMany()
                .HasForeignKey(c => c.AuthorId);

            b.HasOne(c => c.ReplyToTweet)
                .WithMany()
                .HasForeignKey(c => c.ReplyToTweetId);

            b.Navigation(c => c.Author).AutoInclude();

            b.ToTable(TweetData.TableName);
        });
    }

    public IQueryable<T> Get<T>() where T : class
    {
        return Set<T>().AsQueryable();
    }
}

public record ReadOnlyAuthor
{
    public Guid Id { get; }
    public string UserName { get; }
    public string? DisplayName { get; }
    public string? Email { get; }
    public DateTime MemberSince { get; }
}

public record ReadOnlyTweet
{
    public Guid Id { get; }
    public string Message { get; }
    public DateTime PostedOn { get; }
    public Guid AuthorId { get; }
    public ReadOnlyAuthor Author { get; }
    public Guid? ReplyToTweetId { get; }
    public ReadOnlyTweet? ReplyToTweet { get; }
}