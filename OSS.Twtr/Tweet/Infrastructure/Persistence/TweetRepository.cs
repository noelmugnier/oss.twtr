using Microsoft.EntityFrameworkCore;
using OSS.Twtr.Common.Core;
using OSS.Twtr.Tweet.Domain.Aggregates;
using OSS.Twtr.Tweet.Domain.Repositories;
using OSS.Twtr.Tweet.Domain.ValueObjects;

namespace OSS.Twtr.Tweet.Infrastructure.Persistence;

internal class TweetRepository : DbContext, ITweetRepository
{
    public TweetRepository(DbContextOptions<TweetRepository> options) : base(options)
    {
    }

    public async Task<Result<Author>> Get(UserId userId, CancellationToken token)
    {
        try
        {
            var user = await Set<Author>().FirstOrDefaultAsync(c => c.Id == userId, token);
            return user != null
                ? new Result<Author>(user)
                : new Result<Author>(new Error($"User with id {userId} not found", ErrorCode.NotFound));
        }
        catch (Exception e)
        {
            return new Result<Author>(e);
        }
    }

    public async Task<Result<Unit>> Save(Author author, CancellationToken token)
    {
        try
        {
            Set<Author>().Update(author);
            await SaveChangesAsync(token);
            return new Result<Unit>(Unit.Value);
        }
        catch (Exception e)
        {
            return new Result<Unit>(e);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Author>(b =>
        {
            b.HasKey(u => u.Id);
            b.Property(u => u.Id)
                .HasConversion(id => id.Value, guid => UserId.From(guid));

            b.Navigation(u => u.Tweets)
                .HasField("_tweets")
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            b.HasMany(c => c.Tweets)
                .WithOne()
                .HasForeignKey(c => c.AuthorId)
                .IsRequired();

            b.ToTable(UserData.TableName);
        });

        modelBuilder.Entity<Domain.Entities.Tweet>(b =>
        {
            b.HasKey(u => u.Id);
            b.Property(u => u.Id)
                .HasConversion(id => id.Value, guid => TweetId.From(guid));

            b.Property(u => u.Message).IsRequired();
            b.Property(u => u.PostedOn).IsRequired();
            b.Property(u => u.AuthorId).HasColumnName("PostedById").IsRequired();

            b.HasMany<Domain.Entities.Tweet>()
                .WithOne()
                .HasForeignKey(c => c.ReplyToTweetId)
                .IsRequired();

            b.ToTable(TweetData.TableName);
        });
    }
}