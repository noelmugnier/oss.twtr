namespace OSS.Twtr.Core;

public record struct TweetId()
{
    public Guid Value { get; private init; } = Guid.NewGuid();
    public static TweetId New() =>  new (){Value = Guid.NewGuid()};
    public static TweetId From(Guid value) => new (){Value = value};
    public static implicit operator Guid(TweetId d) => d.Value;
    public static explicit operator TweetId(Guid b) => From(b);
}