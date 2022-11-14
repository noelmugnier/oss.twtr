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

    private Tweet(TweetKind kind, string? message, TweetAllowedReplies allowedReplies, UserId authorId, TweetId? referenceTweetId = null, ThreadId? threadId = null) 
        : this()
    {
        Kind = kind;
        Message = message;
        AllowedReplies = allowedReplies;
        PostedOn = DateTime.UtcNow;
        AuthorId = authorId;
        ReferenceTweetId = referenceTweetId;
        ThreadId = threadId;

        if(kind != TweetKind.Reply)
            RaiseEvent(new TweetPosted(Id.Value, authorId.Value));
    }


    public static Tweet Create(string message, UserId authorId, TweetAllowedReplies allowedReplies) => new(TweetKind
    .Tweet, message, allowedReplies, authorId);

    public Tweet Reply(string message, UserId authorId)
    {
        var reply = new Tweet(TweetKind.Reply, message, TweetAllowedReplies.All, authorId, Id);
        RaiseEvent(new TweetReplied(Id.Value,  reply.Id.Value, authorId.Value));
        return reply;
    }

    public Tweet Retweet(UserId authorId)
    {
        var retweet = new Tweet(TweetKind.Retweet, null, TweetAllowedReplies.All, authorId, Id);
        RaiseEvent(new TweetRetweeted(Id.Value, authorId.Value));
        return retweet;
    }

    public Tweet Quote(string? message, UserId authorId, TweetAllowedReplies allowedReplies)
    {
        var quote = new Tweet(TweetKind.Quote, message, allowedReplies, authorId, Id);
        RaiseEvent(new TweetQuoted(Id.Value, authorId.Value));
        return quote;
    }

    public override void Remove()
    {
        RaiseEvent(new TweetRemoved(Id.Value, AuthorId.Value));
    }

    public static Tweet Create(ThreadId threadId, string message, UserId authorId, TweetAllowedReplies allowedReplies)=> new(TweetKind
        .Tweet, message, allowedReplies, authorId, null, threadId);

    public int LikesCount { get; set; }
    public int RetweetsCount { get; set; }

    public TweetKind Kind { get; }
    public string? Message { get; }
    public TweetAllowedReplies AllowedReplies { get; }
    public DateTime PostedOn { get; }
    public UserId AuthorId { get; }
    public TweetId? ReferenceTweetId { get; }
    public Tweet? ReferenceTweet { get; }
    public ThreadId? ThreadId { get; }
}