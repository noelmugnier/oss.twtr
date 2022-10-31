using OSS.Twtr.Common.Domain;

namespace OSS.Twtr.Tweet.Domain.Events;

public record TweetPosted(Guid Id) : DomainEvent;