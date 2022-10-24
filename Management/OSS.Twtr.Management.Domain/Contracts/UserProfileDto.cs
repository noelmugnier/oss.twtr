namespace OSS.Twtr.Management.Domain.Contracts;

public class UserProfileDto
{
    public UserProfileDto(Guid id, string userName, string displayName, DateTimeOffset memberSince, IEnumerable<UserTweetDto> tweets)
    {
        Id = id;
        UserName = userName;
        DisplayName = displayName;
        MemberSince = memberSince;
        Tweets = tweets;
    }

    public Guid Id { get; }
    public string UserName { get; }
    public string DisplayName { get; }
    public DateTimeOffset MemberSince { get; }
    public IEnumerable<UserTweetDto> Tweets { get; }
}