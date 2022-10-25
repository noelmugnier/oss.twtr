namespace OSS.Twtr.Core;

public record DomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTimeOffset RaisedOn { get; } = DateTimeOffset.UtcNow;
}