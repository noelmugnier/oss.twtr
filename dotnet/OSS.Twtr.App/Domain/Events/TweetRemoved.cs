using OSS.Twtr.Domain;

namespace OSS.Twtr.App.Domain.Events;

public record TweetRemoved(Guid TweetId, Guid ByUserId) : DomainEvent;