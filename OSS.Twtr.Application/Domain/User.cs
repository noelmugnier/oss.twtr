using OSS.Twtr.Core;

namespace OSS.Twtr.Domain;

public sealed class User : Entity<UserId>
{
    private User() : this(UserId.New())
    {
    }
    
    public User(UserId id) : base(id)
    {
    }
    
    public string UserName { get; set; }
    public string? DisplayName { get; set; }
    public string? Email { get; set; }
    public DateTime MemberSince { get; set; }
}