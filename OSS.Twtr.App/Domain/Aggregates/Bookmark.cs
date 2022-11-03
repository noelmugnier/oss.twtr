using OSS.Twtr.App.Domain.ValueObjects;
using OSS.Twtr.Domain;

namespace OSS.Twtr.App.Domain.Entities;

public class Bookmark : Aggregate
{
    public UserId UserId { get; }
    public TweetId TweetId { get; }
    public DateTimeOffset BookmarkedOn { get; } = DateTimeOffset.UtcNow;

    private Bookmark(UserId userId, TweetId tweetId)
    {
        UserId = userId;
        TweetId = tweetId;
    }

    public static Bookmark Create(UserId userId, TweetId tweetId)
    {
        return new Bookmark(userId, tweetId);
    }
}