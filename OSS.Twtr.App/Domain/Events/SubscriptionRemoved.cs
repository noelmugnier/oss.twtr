using OSS.Twtr.Domain;

namespace OSS.Twtr.App.Domain.Events;

public record SubscriptionRemoved(Guid SubscribedToUserId, Guid FollowerUserId) : DomainEvent;