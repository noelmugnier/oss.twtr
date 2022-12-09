using OSS.Twtr.Domain;

namespace OSS.Twtr.App.Domain.Events;

public record TweetRetweeted(Guid TweetId, Guid ByUserId) : DomainEvent;