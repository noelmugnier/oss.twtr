using OSS.Twtr.Domain;

namespace OSS.Twtr.App.Domain.Events;

public record TweetBookmarked(Guid TweetId, Guid ByUserId) : DomainEvent;