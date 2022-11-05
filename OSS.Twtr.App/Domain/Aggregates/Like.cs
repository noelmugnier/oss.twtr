using OSS.Twtr.App.Domain.Events;
using OSS.Twtr.App.Domain.ValueObjects;
using OSS.Twtr.Domain;

namespace OSS.Twtr.App.Domain.Entities;

public class Like : Aggregate
{
    public UserId UserId { get; }
    public TweetId TweetId { get; }
    public DateTimeOffset LikedOn { get; } = DateTimeOffset.UtcNow;

    private Like(){}
    
    private Like(UserId userId, TweetId tweetId)
    {
        UserId = userId;
        TweetId = tweetId;
        RaiseEvent(new TweetLiked(tweetId.Value, userId.Value));
    }

    public static Like Create(UserId userId, TweetId tweetId)
    {
        return new Like(userId, tweetId);
    }
}