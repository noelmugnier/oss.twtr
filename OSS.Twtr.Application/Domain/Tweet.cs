using OSS.Twtr.Core;

namespace OSS.Twtr.Domain;

public sealed class Tweet : Entity<TweetId>
{
    private Tweet() : base(TweetId.New())
    {
    }

    public static Tweet Create(string message, User? postedBy)
    {
        if (postedBy == null)
            throw new InvalidOperationException("PostedBy user is not found");
        
        return new Tweet(TweetId.New(), message, postedBy, null);
    }

    private Tweet(TweetId id, string message, User postedBy, TweetId? replyToTweetId)
        : base(id)
    {
        Message = message;
        PostedOn = DateTime.UtcNow;
        PostedBy = postedBy;
        ReplyToTweetId = replyToTweetId;
    }

    public string Message { get; }
    public DateTime PostedOn { get; }
    public User PostedBy { get; }
    public TweetId? ReplyToTweetId { get; }

    public Tweet Reply(string message, User postedBy)
    {
        if (postedBy == null)
            throw new InvalidOperationException("PostedBy user is not found");
        
        return new Tweet(TweetId.New(), message, postedBy, Id);
    }
}