namespace OSS.Twtr.Domain;

public abstract class Aggregate<TId> : Entity<TId>, IEquatable<Aggregate<TId>> where TId : IdentifiableId
{
    private List<DomainEvent> _events = new();

    protected void RaiseEvent(DomainEvent @event) => _events.Add(@event);

    public IReadOnlyCollection<DomainEvent> DomainEvents => _events.AsReadOnly();
    
    protected Aggregate(TId id) : base(id){}
    public bool Equals(Aggregate<TId>? other)
    {
        return Equals((object?) other);
    }
}

public abstract class AggregateRoot<TId> : Aggregate<TId>, IEquatable<AggregateRoot<TId>> where TId : IdentifiableId
{
    protected AggregateRoot(TId id) : base(id){}
    public bool Equals(AggregateRoot<TId>? other)
    {
        return Equals((object?) other);
    }
}