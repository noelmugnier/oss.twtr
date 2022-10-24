namespace OSS.Twtr.Management.Domain.Contracts;

public record struct UserProfileDto(Guid Id, string UserName, string DisplayName, DateTimeOffset MemberSince, IEnumerable<UserTweetDto> Tweets);