using OSS.Twtr.Domain;

namespace OSS.Twtr.Management.Domain;

public class Tweet : Entity
{
    public Tweet(string message, User user)
        : this(TweetId.New(), message, DateTimeOffset.UtcNow, user)
    {
        RaiseEvent(new TweetPosted(Id));
    }

    public Tweet(TweetId id, string message, DateTimeOffset postedOn, User user)
    {
        Id = id;
        Message = message;
        PostedOn = postedOn;
        User = user;
        UserId = user.Id;
    }

    public TweetId Id { get; }
    public string Message { get; }
    public DateTimeOffset PostedOn { get; }
    public UserId UserId { get; }
    public User User { get; }
}