using OSS.Twtr.Domain.Ids;

namespace OSS.Twtr.Domain.Events;

public record AccountCreated(AccountId AccountId, string UserName) : DomainEvent;