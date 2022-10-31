namespace OSS.Twtr.Common.Domain;

public abstract class ValueObject: DomainObject, IEquatable<ValueObject>
{
    public abstract string GetValue();
    protected abstract IEnumerable<object> GetEqualityComponents();
    
    public bool Equals(ValueObject? other)
    {
        return Equals((object?) other);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        
        var valueObject = (ValueObject) obj;
        return GetEqualityComponents().SequenceEqual(valueObject.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x?.GetHashCode() ?? 0)
            .Aggregate((x, y) => x ^ y);
    }

    public static bool operator ==(ValueObject? left, ValueObject? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(ValueObject? left, ValueObject? right)
    {
        return !Equals(left, right);
    }
}

public abstract class ValueObject<T> : ValueObject
{
    public T Value { get; protected init; }

    protected ValueObject(T value)
    {
        Value = value;
    }

    public override string GetValue()
    {
        return Value?.ToString();
    }
}