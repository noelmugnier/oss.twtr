namespace OSS.Twtr.Core;

public class TweetId : ValueObject, IdentifiableId, IEquatable<TweetId>
{
    public Guid Value { get; private init; } = Guid.NewGuid();
    public static TweetId New() => new() {Value = Guid.NewGuid()};
    public static TweetId From(Guid value) => new() {Value = value};

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public bool Equals(TweetId? other)
    {
        return Equals((object?) other);
    }
}