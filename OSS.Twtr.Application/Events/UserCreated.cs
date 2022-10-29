namespace OSS.Twtr.Core;

public record UserCreated(Guid UserId) : DomainEvent;