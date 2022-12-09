using OSS.Twtr.Domain;

namespace OSS.Twtr.App.Domain.ValueObjects;

public class ThreadId : ValueObject<Guid>, IdentifiableId, IEquatable<ThreadId>
{
    private ThreadId() : base(Guid.NewGuid()) {}
    
    private ThreadId(Guid value) : base(value) {}
    
    public static ThreadId New() => new();
    public static ThreadId None => From(Guid.Empty);
    public static ThreadId From(Guid value) => new(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public bool Equals(ThreadId? other)
    {
        return Equals((object?) other);
    }
}