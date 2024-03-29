﻿using Microsoft.EntityFrameworkCore;
using OSS.Twtr.App.Domain.Entities;
using OSS.Twtr.App.Domain.Enums;
using OSS.Twtr.Application;

namespace OSS.Twtr.App.Infrastructure;

internal sealed class ReadDbContext : DbContext
{
    public ReadDbContext(DbContextOptions<ReadDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<ReadOnlyUser>(b =>
        {
            b.HasKey(u => u.Id);

            b.Property(c => c.MemberSince);
            b.Property(c => c.UserName);
            b.Property(c => c.DisplayName);
            b.Property(t => t.Job);
            b.Property(t => t.Description);
            b.Property(t => t.BirthDate);
            b.Property(t => t.Url);
            b.Property(t => t.Location);
            
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
            b.Property(c => c.LikesCount);
            b.Property(c => c.RetweetsCount);

            b.HasOne(c => c.Author)
                .WithMany()
                .HasForeignKey(c => c.AuthorId);
            
            b.HasMany<ReadOnlyLike>()
                .WithOne()
                .HasForeignKey(c => c.TweetId);

            b.HasOne(c => c.ReferenceTweet)
                .WithMany()
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
            b.ToTable("BlockedUsers");
        });

        modelBuilder.Entity<ReadOnlyMute>(b =>
        {
            b.HasKey(u => new {u.UserId, u.UserIdToMute});
            b.ToTable("MutedUsers");
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
}

public record ReadOnlyUser
{
    public Guid Id { get; }
    public string UserName { get; }
    public string? DisplayName { get; }
    public DateTime MemberSince { get; }
    public string? Job { get; }
    public string? Description { get; }
    public DateTime? BirthDate { get; }
    public string? Location { get; }
    public string? Url { get; }
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
    public ReadOnlyUser Author { get; }
    public Guid? ReferenceTweetId { get; }
    public Guid? ThreadId { get; }
    public ReadOnlyTweet? ReferenceTweet { get; }
    public int LikesCount { get; }
    public int RetweetsCount { get; }
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