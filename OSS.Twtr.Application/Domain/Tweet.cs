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
        
        return new Tweet(TweetId.New(), message, DateTimeOffset.UtcNow.DateTime, postedBy);
    }

    private Tweet(TweetId id, string message, DateTime postedOn, User postedBy)
        : base(id)
    {
        Message = message;
        PostedOn = postedOn;
        PostedBy = postedBy;
    }

    public string Message { get; }
    public DateTime PostedOn { get; }
    public User PostedBy { get; }
}