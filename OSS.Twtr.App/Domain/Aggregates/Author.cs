using OSS.Twtr.App.Domain.Events;
using OSS.Twtr.App.Domain.ValueObjects;
using OSS.Twtr.Domain;

namespace OSS.Twtr.App.Domain.Aggregates;

public class Author : AggregateRoot<UserId>
{
    private List<Entities.Tweet> _tweets = new();

    private Author() : base(UserId.None)
    {
    }

    public IReadOnlyCollection<Entities.Tweet> Tweets => _tweets.AsReadOnly();

    public TweetId Post(string message)
    {
        var tweet = Entities.Tweet.Create(message, Id);
        _tweets.Add(tweet);

        RaiseEvent(new TweetPosted(tweet.Id.Value));
        return tweet.Id;
    }

    public TweetId ReplyTo(TweetId tweetIdToReplyTo, string message)
    {
        var tweet = Entities.Tweet.ReplyTo(tweetIdToReplyTo, message, Id);
        _tweets.Add(tweet);

        RaiseEvent(new TweetReplied(tweet.Id.Value));
        return tweet.Id;
    }
}