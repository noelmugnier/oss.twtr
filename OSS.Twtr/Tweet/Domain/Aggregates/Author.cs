using OSS.Twtr.Common.Domain;
using OSS.Twtr.Tweet.Domain.Events;
using OSS.Twtr.Tweet.Domain.ValueObjects;

namespace OSS.Twtr.Tweet.Domain.Aggregates;

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