using Microsoft.EntityFrameworkCore;
using OSS.Twtr.App.Domain.Entities;
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
        modelBuilder.Entity<Author>(b =>
        {
            b.HasKey(u => u.Id);
            b.Property(u => u.Id)
                .HasConversion(id => id.Value, guid => UserId.From(guid));

            b.Property(t => t.UserName).IsRequired();
            b.Property(t => t.DisplayName);
            b.Property(t => t.Email);
            b.Property(t => t.MemberSince).IsRequired();

            b.Ignore(c => c.DomainEvents);
            
            b.ToTable("Users");
        });

        modelBuilder.Entity<Tweet>(b =>
        {
            b.HasKey(u => u.Id);
            b.Property(u => u.Id)
                .HasConversion(id => id.Value, guid => TweetId.From(guid));

            b.Property(t => t.Kind).IsRequired();
            b.Property(t => t.Message);
            b.Property(t => t.PostedOn).IsRequired();
            b.Property(t => t.AuthorId).IsRequired();
            b.Property(t => t.ThreadId)
                .HasConversion(id => id.Value, guid => ThreadId.From(guid));

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
            
            b.Ignore(c => c.DomainEvents);
            
            b.ToTable("Bookmarks");
        });
    }
}
