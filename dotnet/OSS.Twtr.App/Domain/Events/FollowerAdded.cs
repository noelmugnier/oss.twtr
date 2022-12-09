using OSS.Twtr.Domain;

namespace OSS.Twtr.App.Domain.Events;

public record FollowerAdded(Guid SubscribedToUserId, Guid FollowerUserId) : DomainEvent;