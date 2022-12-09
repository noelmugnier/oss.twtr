using OSS.Twtr.Domain;

namespace OSS.Twtr.App.Domain.Events;

public record UserMutedBy(Guid MutedUserId, Guid ByUserId) : DomainEvent;