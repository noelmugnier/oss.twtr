using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using OSS.Twtr.App.Domain.Entities;
using OSS.Twtr.App.Domain.Enums;
using OSS.Twtr.App.Domain.ValueObjects;
using OSS.Twtr.Domain;

namespace OSS.Twtr.App.Infrastructure;

public sealed class AppDbContext : DbContext
{
    private readonly IEventDispatcher _dispatcher;

    public AppDbContext(DbContextOptions<AppDbContext> options, IEventDispatcher dispatcher) : base(options)
    {
        _dispatcher = dispatcher;
    }

    public override EntityEntry<TEntity> Remove<TEntity>(TEntity entity)
    {
        if (entity is IAggregate aggregate)
            aggregate.Remove();

        return base.Remove(entity);
    }

    public override EntityEntry Remove(object entity)
    {
        if (entity is IAggregate aggregate)
            aggregate.Remove();

        return base.Remove(entity);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        var result = await base.SaveChangesAsync(cancellationToken);
        
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is not IAggregate aggregate) 
                continue;
            
            foreach (var @event in aggregate.DomainEvents)
                _dispatcher.Dispatch(@event);
            
            aggregate.ClearEvents();
        }

        return result;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Author>(b =>
        {
            b.HasKey(u => u.Id);
            b.Property(u => u.Id)
                .HasConversion(id => id.Value, guid => UserId.From(guid));

            b.Property(t => t.UserName).IsRequired();
            b.Property(t => t.DisplayName);
            b.Property(t => t.Email);
            b.Property(t => t.MemberSince).IsRequired();
            b.Property(t => t.PinnedTweetId)
                .HasConversion(
                    id => id != null ? id.Value : (Guid?) null, 
                    guid => guid != null ? TweetId.From(guid.Value) : null);

            b.HasOne<Tweet>().WithOne().HasForeignKey<Author>(c => c.PinnedTweetId);

            b.Ignore(c => c.DomainEvents);

            b.ToTable("Users");
        });

        modelBuilder.Entity<Tweet>(b =>
        {
            b.HasKey(u => u.Id);
            b.Property(u => u.Id)
                .HasConversion(id => id.Value, guid => TweetId.From(guid));

            b.Property(t => t.Kind).IsRequired();
            b.Property(t => t.AllowedReplies).IsRequired().HasDefaultValue(TweetAllowedReplies.All);
            b.Property(t => t.Message);
            b.Property(t => t.PostedOn).IsRequired();
            b.Property(t => t.AuthorId).IsRequired();
            b.Property(t => t.ThreadId)
                .HasConversion(
                    id => id != null ? id.Value : (Guid?) null, 
                    guid => guid != null ? ThreadId.From(guid.Value) : null);
            b.Property(t => t.ReferenceTweetId)
                .HasConversion(
                    id => id != null ? id.Value : (Guid?) null, 
                    guid => guid != null ? TweetId.From(guid.Value) : null);

            b.HasOne<Author>()
                .WithMany()
                .HasForeignKey(c => c.AuthorId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            b.HasOne<Tweet>()
                .WithMany()
                .HasForeignKey(c => c.ReferenceTweetId)
                .OnDelete(DeleteBehavior.Restrict);

            b.Ignore(c => c.DomainEvents);

            b.ToTable("Tweets");
        });

        modelBuilder.Entity<Subscription>(b =>
        {
            b.Property(c => c.SubscribedToUserId)
                .HasConversion(id => id.Value, guid => UserId.From(guid));

            b.Property(c => c.FollowerUserId)
                .HasConversion(id => id.Value, guid => UserId.From(guid));

            b.Property(c => c.SubscribedOn);

            b.HasKey(c => new {c.FollowerUserId, c.SubscribedToUserId});

            b.Ignore(c => c.DomainEvents);
            b.ToTable("Subscriptions");
        });

        modelBuilder.Entity<Like>(b =>
        {
            b.Property(c => c.UserId)
                .HasConversion(id => id.Value, guid => UserId.From(guid));

            b.Property(c => c.TweetId)
                .HasConversion(id => id.Value, guid => TweetId.From(guid));

            b.Property(c => c.LikedOn);
            b.HasKey(c => new {c.UserId, c.TweetId});

            b.HasOne<Author>().WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne<Tweet>().WithMany()
                .HasForeignKey(c => c.TweetId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Ignore(c => c.DomainEvents);
            b.ToTable("Likes");
        });

        modelBuilder.Entity<Bookmark>(b =>
        {
            b.Property(c => c.UserId)
                .HasConversion(id => id.Value, guid => UserId.From(guid));

            b.Property(c => c.TweetId)
                .HasConversion(id => id.Value, guid => TweetId.From(guid));

            b.Property(c => c.BookmarkedOn);
            b.HasKey(c => new {c.UserId, c.TweetId});

            b.HasOne<Author>().WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne<Tweet>().WithMany()
                .HasForeignKey(c => c.TweetId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Ignore(c => c.DomainEvents);
            b.ToTable("Bookmarks");
        });

        modelBuilder.Entity<Block>(b =>
        {
            b.Property(c => c.UserId)
                .HasConversion(id => id.Value, guid => UserId.From(guid));

            b.Property(c => c.UserIdToBlock)
                .HasConversion(id => id.Value, guid => UserId.From(guid));

            b.Property(c => c.BlockedOn);
            b.HasKey(c => new {c.UserId, c.UserIdToBlock});

            b.HasOne<Author>().WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne<Author>().WithMany()
                .HasForeignKey(c => c.UserIdToBlock)
                .OnDelete(DeleteBehavior.Cascade);

            b.Ignore(c => c.DomainEvents);
            b.ToTable("BlockedUsers");
        });

        modelBuilder.Entity<Mute>(b =>
        {
            b.Property(c => c.UserId)
                .HasConversion(id => id.Value, guid => UserId.From(guid));

            b.Property(c => c.UserIdToMute)
                .HasConversion(id => id.Value, guid => UserId.From(guid));

            b.Property(c => c.MutedOn);
            b.HasKey(c => new {c.UserId, c.UserIdToMute});

            b.HasOne<Author>().WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne<Author>().WithMany()
                .HasForeignKey(c => c.UserIdToMute)
                .OnDelete(DeleteBehavior.Cascade);

            b.Ignore(c => c.DomainEvents);
            b.ToTable("MutedUsers");
        });
    }
}