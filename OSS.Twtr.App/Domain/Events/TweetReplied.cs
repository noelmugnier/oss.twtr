using OSS.Twtr.Domain;

namespace OSS.Twtr.App.Domain.Events;

public record TweetReplied(Guid TweetId, Guid ByUserId) : DomainEvent;