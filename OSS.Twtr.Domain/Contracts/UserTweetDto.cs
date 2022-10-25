namespace OSS.Twtr.Domain.Contracts;

public record UserTweetDto(Guid Id, string Message, DateTimeOffset PostedOn);