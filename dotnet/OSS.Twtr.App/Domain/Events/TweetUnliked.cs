using OSS.Twtr.Domain;

namespace OSS.Twtr.App.Domain.Events;

public record TweetUnliked(Guid TweetId, Guid ByUserId) : DomainEvent;