namespace OSS.Twtr.Core;

public abstract class Entity
{
    private List<DomainEvent> _events = new();

    protected void RaiseEvent(DomainEvent @event) => _events.Add(@event);

    public IReadOnlyCollection<DomainEvent> DomainEvents => _events.AsReadOnly();

}