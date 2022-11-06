namespace OSS.Twtr.Domain;

public interface IAggregate
{
    IReadOnlyCollection<DomainEvent> DomainEvents { get; }
    void ClearEvents();
    void Remove();
}

public abstract class Aggregate : IEquatable<Aggregate>, IAggregate
{
    private List<DomainEvent> _events = new();

    protected void RaiseEvent(DomainEvent @event) => _events.Add(@event);

    public IReadOnlyCollection<DomainEvent> DomainEvents => _events.AsReadOnly();
    public void ClearEvents()
    {
        _events.Clear();
    }

    public abstract void Remove();

    public bool Equals(Aggregate? other)
    {
        return Equals((object?) other);
    }
}

public abstract class Aggregate<TId> : Entity<TId>, IAggregate, IEquatable<Aggregate<TId>> where TId : IdentifiableId
{
    private List<DomainEvent> _events = new();

    protected void RaiseEvent(DomainEvent @event) => _events.Add(@event);

    public IReadOnlyCollection<DomainEvent> DomainEvents => _events.AsReadOnly();
    public void ClearEvents()
    {
        _events.Clear();
    }
    
    public abstract void Remove();

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