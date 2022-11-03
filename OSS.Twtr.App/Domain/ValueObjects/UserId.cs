using OSS.Twtr.Domain;

namespace OSS.Twtr.App.Domain.ValueObjects;

public class UserId : ValueObject<Guid>, IdentifiableId, IEquatable<UserId>
{
    private UserId() : this(Guid.NewGuid()) {}
    
    private UserId(Guid value) : base(value) {}
    
    public static UserId New() => new();
    public static UserId None => From(Guid.Empty);
    public static UserId From(Guid value) => new(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public bool Equals(UserId? other)
    {
        return Equals((object?)other);
    }
}