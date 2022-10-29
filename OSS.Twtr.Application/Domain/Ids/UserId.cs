namespace OSS.Twtr.Core;

public class UserId : ValueObject, IdentifiableId, IEquatable<UserId>
{
    public Guid Value { get; private init; } = Guid.NewGuid();
    public static UserId From(Guid value) => new() {Value = value};
    public static UserId New() => new() {Value = Guid.NewGuid()};
    // public static implicit operator Guid(UserId d) => d.Value;
    // public static explicit operator UserId(Guid b) => From(b);
    // public static implicit operator string(UserId d) => d.Value.ToString("D");
    // public static explicit operator UserId(string b) => From(Guid.Parse(b));

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public bool Equals(UserId? other)
    {
        return Equals((object?)other);
    }
}