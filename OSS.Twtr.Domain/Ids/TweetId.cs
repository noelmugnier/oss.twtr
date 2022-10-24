namespace OSS.Twtr.Domain;

public record struct TweetId()
{
    public Guid Value { get; private init; } = Guid.NewGuid();
    public static TweetId New() =>  new (){Value = Guid.NewGuid()};
    public static TweetId From(Guid value) => new (){Value = value};
}