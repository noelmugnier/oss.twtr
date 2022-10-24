namespace OSS.Twtr.Management.Domain.Contracts;

public record struct UserTweetDto(Guid Id, string Message, DateTimeOffset PostedOn);