namespace OSS.Twtr.Domain;

public record DomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTimeOffset RaisedOn { get; } = DateTimeOffset.UtcNow;
}