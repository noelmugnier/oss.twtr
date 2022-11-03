namespace OSS.Twtr.Domain;

public abstract record DomainEvent : DomainObject
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTimeOffset RaisedOn { get; } = DateTimeOffset.UtcNow;
}