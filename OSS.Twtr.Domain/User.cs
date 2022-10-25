using OSS.Twtr.Core;

namespace OSS.Twtr.Domain;

public class User : Entity
{
    public User(UserId id, string userName, DateTimeOffset memberSince, string? displayName = null)
    {
        Id = id;
        UserName = userName;
        DisplayName = displayName ?? userName;
        MemberSince = memberSince;
    }

    public UserId Id { get; }
    public string UserName { get; }
    public string DisplayName { get; }
    public DateTimeOffset MemberSince { get; }
}