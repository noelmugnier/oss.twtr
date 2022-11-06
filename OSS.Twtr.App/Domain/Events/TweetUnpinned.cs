using OSS.Twtr.Domain;

namespace OSS.Twtr.App.Domain.Events;

public record TweetUnpinned(Guid TweetId, Guid ByUserId) : DomainEvent;