using OSS.Twtr.Domain;

namespace OSS.Twtr.App.Domain.Events;

public record TweetUnbookmarked(Guid TweetId, Guid ByUserId) : DomainEvent;