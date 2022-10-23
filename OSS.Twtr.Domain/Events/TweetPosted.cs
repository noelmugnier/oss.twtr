using OSS.Twtr.Domain.Ids;

namespace OSS.Twtr.Domain.Events;

public record TweetPosted(TweetId Id) : DomainEvent;