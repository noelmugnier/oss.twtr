using OSS.Twtr.App.Domain.Enums;
using OSS.Twtr.App.Domain.Events;
using OSS.Twtr.App.Domain.ValueObjects;
using OSS.Twtr.Domain;

namespace OSS.Twtr.App.Domain.Entities;

public class Tweet : Aggregate<TweetId>
{
    private Tweet() : base(TweetId.New())
    {
    }

    private Tweet(TweetKind kind, string? message, UserId authorId, TweetId? referenceTweetId = null, ThreadId? threadId = null) 
        : this()
    {
        Kind = kind;
        Message = message;
        PostedOn = DateTime.UtcNow;
        AuthorId = authorId;
        ReferenceTweetId = referenceTweetId;
        ThreadId = threadId;

        if(threadId == null)
            RaiseEvent(new TweetPosted(Id.Value, authorId.Value));
    }


    public static Tweet Create(string message, UserId authorId) => new(TweetKind.Tweet, message, authorId);

    public Tweet Reply(string message, UserId authorId)
    {
        var reply = new Tweet(TweetKind.Reply, message, authorId, Id);
        RaiseEvent(new TweetReplied(Id.Value, authorId.Value));
        return reply;
    }

    public Tweet Retweet(UserId authorId)
    {
        var retweet = new Tweet(TweetKind.Retweet, null, authorId, Id);
        RaiseEvent(new TweetRetweeted(Id.Value, authorId.Value));
        return retweet;
    }

    public Tweet Quote(string? message, UserId authorId)
    {
        var quote = new Tweet(TweetKind.Quote, message, authorId, Id);
        RaiseEvent(new TweetQuoted(Id.Value, authorId.Value));
        return quote;
    }

    public static Tweet Create(ThreadId threadId, string message, UserId authorId)
    {
        var tweet = new Tweet(TweetKind.Tweet, message, authorId, null, threadId);
        return tweet;
    }

    public TweetKind Kind { get; }
    public string? Message { get; }
    public DateTime PostedOn { get; }
    public UserId AuthorId { get; }
    public TweetId? ReferenceTweetId { get; }
    public ThreadId? ThreadId { get; }
}