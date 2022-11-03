using Microsoft.EntityFrameworkCore;
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

            b.HasOne(c => c.ReferenceTweet)
                .WithMany()
                .HasForeignKey(c => c.ReferenceTweetId);

            b.Navigation(c => c.Author).AutoInclude();

            b.ToTable("Tweets");
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
    public TweetKind Kind { get; }
    public string? Message { get; }
    public DateTime PostedOn { get; }
    public Guid AuthorId { get; }
    public ReadOnlyAuthor Author { get; }
    public Guid? ReferenceTweetId { get; }
    public Guid? ThreadId { get; }
    public ReadOnlyTweet? ReferenceTweet { get; }
}