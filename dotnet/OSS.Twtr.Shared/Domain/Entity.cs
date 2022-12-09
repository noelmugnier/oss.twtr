namespace OSS.Twtr.Domain;

public abstract class Entity<TId> : IdentifiableDomainObject<TId>, IEquatable<Entity<TId>> where TId : IdentifiableId
{
    protected Entity(TId id) : base(id){}
    public bool Equals(Entity<TId>? other)
    {
        return Equals((object?) other);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;

        return obj is Entity<TId> entity && Id.Equals(entity.Id);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public static bool operator ==(Entity<TId>? left, Entity<TId>? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Entity<TId>? left, Entity<TId>? right)
    {
        return !Equals(left, right);
    }
}