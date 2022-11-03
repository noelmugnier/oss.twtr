using OSS.Twtr.Domain;

namespace OSS.Twtr.App.Domain.Events;

public record TweetPosted(Guid Id) : DomainEvent;