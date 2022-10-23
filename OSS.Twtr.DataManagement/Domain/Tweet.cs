using OSS.Twtr.Domain;
using OSS.Twtr.Domain.Events;
using OSS.Twtr.Domain.Ids;

namespace OSS.Twtr.TweetManagement.Domain;

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
    }

    public TweetId Id { get; }
    public string Message { get; }
    public DateTimeOffset PostedOn { get; }
    public User User { get; }
}