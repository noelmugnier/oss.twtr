namespace OSS.Twtr.Core;

public record UserConnected(Guid UserId) : DomainEvent;