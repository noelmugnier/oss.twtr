namespace OSS.Twtr.Domain;

public record AccountCreated(AccountId AccountId, string UserName) : DomainEvent;