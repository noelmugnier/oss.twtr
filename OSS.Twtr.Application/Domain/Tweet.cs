using OSS.Twtr.Core;

namespace OSS.Twtr.Domain;

public sealed class Tweet : Entity<TweetId>
{
    private Tweet() : base(TweetId.New())
    {
    }

    public static Tweet Create(string message, UserId postedById)
    {
        return new Tweet(TweetId.New(), message, postedById, null);
    }

    private Tweet(TweetId id, string message, UserId postedById, TweetId? replyToTweetId)
        : base(id)
    {
        Message = message;
        PostedOn = DateTime.UtcNow;
        PostedById = postedById;
        ReplyToTweetId = replyToTweetId;
    }

    public string Message { get; }
    public DateTime PostedOn { get; }
    public UserId PostedById { get; }
    public TweetId? ReplyToTweetId { get; }

    public static Tweet ReplyTo(string message, TweetId replyToTweetId, UserId postedById)
    {
        return new Tweet(TweetId.New(), message, postedById, replyToTweetId);
    }
}