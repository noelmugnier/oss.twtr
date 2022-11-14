using OSS.Twtr.Domain;

namespace OSS.Twtr.App.Domain.Events;

public record TweetReplied(Guid RepliedTweetId, Guid ReplyTweetId, Guid ByUserId) : DomainEvent;