using OSS.Twtr.Common.Domain;
using OSS.Twtr.Tweet.Domain.ValueObjects;

namespace OSS.Twtr.Tweet.Domain.Entities;

public class Tweet : Entity<TweetId>
{
    private Tweet() : base(TweetId.New())
    {
    }

    private Tweet(string message, UserId authorId, TweetId? replyToTweetId = null) : this()
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new InvalidOperationException("Tweet cannot be empty");

        Message = message;
        PostedOn = DateTime.UtcNow;
        AuthorId = authorId;
        ReplyToTweetId = replyToTweetId;
    }

    internal static Tweet Create(string message, UserId authorId) => new(message, authorId);

    internal static Tweet ReplyTo(TweetId tweetIdToReply, string message, UserId authorId) =>
        new(message, authorId, tweetIdToReply);

    public string Message { get; } = null!;
    public DateTime PostedOn { get; }
    public UserId AuthorId { get; }
    public TweetId? ReplyToTweetId { get; }
}