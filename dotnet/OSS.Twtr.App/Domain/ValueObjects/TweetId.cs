using OSS.Twtr.Domain;

namespace OSS.Twtr.App.Domain.ValueObjects;

public class TweetId : ValueObject<Guid>, IdentifiableId, IEquatable<TweetId>
{
    private TweetId() : base(Guid.NewGuid()) {}
    
    private TweetId(Guid value) : base(value) {}
    
    public static TweetId New() => new();
    public static TweetId None => From(Guid.Empty);
    public static TweetId From(Guid value) => new(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public bool Equals(TweetId? other)
    {
        return Equals((object?) other);
    }
}