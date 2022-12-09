using OSS.Twtr.Domain;

namespace OSS.Twtr.App.Domain.Events;

public record UserBlockedBy(Guid BlockedUserId, Guid ByUserId) : DomainEvent;