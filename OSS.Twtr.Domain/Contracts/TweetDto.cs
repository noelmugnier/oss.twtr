namespace OSS.Twtr.Domain.Contracts;

public record TweetDto(Guid Id, string Message, DateTimeOffset PostedOn, Guid UserId, UserDto User);