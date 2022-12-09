using OSS.Twtr.App.Domain.Enums;
using OSS.Twtr.App.Domain.ValueObjects;

namespace OSS.Twtr.App.Domain.Entities;

public class Token
{
    public Token(string value, TweetId tweetId)
    {
        Id = Guid.NewGuid();
        Value = value.Trim();
        TweetId = tweetId;
        CreatedOn = DateTime.UtcNow;
        
        if(Value.StartsWith('@'))
            Kind=TokenKind.Mention;
        else if(Value.StartsWith('#'))
            Kind=TokenKind.Hashtag;
        else
            Kind=TokenKind.Word;
    }

    public Guid Id { get; }
    public TokenKind Kind { get; }
    public TweetId TweetId { get; }
    public string Value { get; }
    public DateTime CreatedOn { get; }
}