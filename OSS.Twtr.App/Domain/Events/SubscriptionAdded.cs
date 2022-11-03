using OSS.Twtr.Domain;

namespace OSS.Twtr.App.Domain.Events;

public record SubscriptionAdded(Guid SubscribedToUserId, Guid FollowerUserId) : DomainEvent;