using OSS.Twtr.App.Domain.Events;
using OSS.Twtr.App.Domain.ValueObjects;
using OSS.Twtr.Domain;

namespace OSS.Twtr.App.Domain.Entities;

public class Bookmark : Aggregate
{
    public UserId UserId { get; }
    public TweetId TweetId { get; }
    public DateTimeOffset BookmarkedOn { get; } = DateTimeOffset.UtcNow;

    private Bookmark(){}
    
    private Bookmark(UserId userId, TweetId tweetId)
    {
        UserId = userId;
        TweetId = tweetId;
        RaiseEvent(new TweetBookmarked(tweetId.Value, userId.Value));
    }

    public static Bookmark Create(UserId userId, TweetId tweetId)
    {
        return new Bookmark(userId, tweetId);
    }

    public override void Remove()
    {
        RaiseEvent(new TweetUnbookmarked(TweetId.Value, UserId.Value));
    }
}