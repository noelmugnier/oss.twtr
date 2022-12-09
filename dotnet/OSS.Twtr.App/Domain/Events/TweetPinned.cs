using OSS.Twtr.Domain;

namespace OSS.Twtr.App.Domain.Events;

public record TweetPinned(Guid TweetId, Guid ByUserId) : DomainEvent;