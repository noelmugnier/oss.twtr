namespace OSS.Twtr.Management.Domain.Contracts;

public record struct TweetDto(Guid Id, string Message, DateTimeOffset PostedOn, UserDto User);