using Microsoft.EntityFrameworkCore;
using OSS.Twtr.App.Domain.Entities;
using OSS.Twtr.App.Domain.Enums;
using OSS.Twtr.Application;

namespace OSS.Twtr.App.Infrastructure;

internal sealed class ReadDbContext : DbContext, IReadDbContext
{
    public ReadDbContext(DbContextOptions<ReadDbContext> options) : base(options)
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
            
            b.ToTable("Users");
        });

        modelBuilder.Entity<ReadOnlyTweet>(b =>
        {
            b.HasKey(u => u.Id);
            
            b.Property(c => c.Kind);
            b.Property(c => c.AuthorId);
            b.Property(c => c.Message);
            b.Property(c => c.PostedOn);
            b.Property(c => c.ThreadId);

            b.HasOne(c => c.Author)
                .WithMany()
                .HasForeignKey(c => c.AuthorId);
            
            b.HasMany(c => c.Likes)
                .WithOne()
                .HasForeignKey(c => c.TweetId);

            b.HasOne(c => c.ReferenceTweet)
                .WithMany(c => c.Retweets)
                .HasForeignKey(c => c.ReferenceTweetId);

            b.Navigation(c => c.Author).AutoInclude();

            b.ToTable("Tweets");
        });

        modelBuilder.Entity<ReadOnlyTrend>(b =>
        {
            b.HasKey(u => new {u.AnalyzedOn, u.Name});
            b.Property(c => c.TweetCount);
            b.ToTable("Trendings");
        });

        modelBuilder.Entity<ReadOnlyLike>(b =>
        {
            b.HasKey(u => new {u.UserId, u.TweetId});
            b.ToTable("Likes");
        });

        modelBuilder.Entity<ReadOnlyBlock>(b =>
        {
            b.HasKey(u => new {u.UserId, u.UserIdToBlock});
            b.ToTable("Blocks");
        });

        modelBuilder.Entity<ReadOnlyMute>(b =>
        {
            b.HasKey(u => new {u.UserId, u.UserIdToMute});
            b.ToTable("Mutes");
        });

        modelBuilder.Entity<ReadOnlySubscription>(b =>
        {
            b.HasKey(u => new {u.FollowerUserId, u.SubscribedToUserId});
            b.ToTable("Subscriptions");
        });

        modelBuilder.Entity<ReadOnlyBookmark>(b =>
        {
            b.HasKey(u => new {u.UserId, u.TweetId});
            b.HasOne(c => c.Tweet)
                .WithMany()
                .HasForeignKey(c => c.TweetId);
            
            b.ToTable("Bookmarks");
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

public record ReadOnlySubscription
{
    public Guid FollowerUserId { get; }
    public Guid SubscribedToUserId { get; }
}

public record ReadOnlyTweet
{
    public Guid Id { get; }
    public TweetKind Kind { get; }
    public string? Message { get; }
    public DateTime PostedOn { get; }
    public Guid AuthorId { get; }
    public ReadOnlyAuthor Author { get; }
    public Guid? ReferenceTweetId { get; }
    public Guid? ThreadId { get; }
    public ReadOnlyTweet? ReferenceTweet { get; }
    public ICollection<ReadOnlyLike> Likes { get; }
    public ICollection<ReadOnlyTweet> Retweets { get; }
}

public record ReadOnlyTrend
{
    public DateTime AnalyzedOn { get; set; }
    public string Name { get; set; }
    public int TweetCount { get; set; }
}

public record ReadOnlyLike
{
    public Guid UserId { get; }
    public Guid TweetId { get; }
}

public record ReadOnlyBlock
{
    public Guid UserId { get; }
    public Guid UserIdToBlock { get; }
}

public record ReadOnlyMute
{
    public Guid UserId { get; }
    public Guid UserIdToMute { get; }
}

public record ReadOnlyBookmark
{
    public Guid UserId { get; }
    public Guid TweetId { get; }
    public ReadOnlyTweet Tweet { get; }
}