namespace OSS.Twtr.Management.Domain.Contracts;

public record UserTweetDto(Guid Id, string Message, DateTimeOffset PostedOn);