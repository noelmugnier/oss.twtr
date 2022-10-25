namespace OSS.Twtr.Core;

public record AccountCreated(UserId UserId, string UserName) : DomainEvent;
public record UserConnected(UserId UserId) : DomainEvent;