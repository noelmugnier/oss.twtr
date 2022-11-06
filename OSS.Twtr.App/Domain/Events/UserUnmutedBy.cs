using OSS.Twtr.Domain;

namespace OSS.Twtr.App.Domain.Events;

public record UserUnmutedBy(Guid UnmutedUserId, Guid ByUserId) : DomainEvent;