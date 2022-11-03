using OSS.Twtr.Domain;

namespace OSS.Twtr.App.Domain.Events;

public record TweetQuoted(Guid TweetId, Guid ByUserId) : DomainEvent;