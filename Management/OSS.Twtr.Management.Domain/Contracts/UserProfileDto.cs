namespace OSS.Twtr.Management.Domain.Contracts;

public record UserProfileDto(Guid Id, string UserName, string DisplayName, DateTimeOffset MemberSince, IEnumerable<UserTweetDto> Tweets);