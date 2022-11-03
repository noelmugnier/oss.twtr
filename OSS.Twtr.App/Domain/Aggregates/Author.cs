using OSS.Twtr.App.Domain.ValueObjects;
using OSS.Twtr.Domain;

namespace OSS.Twtr.App.Domain.Entities;

public class Author : Aggregate<UserId>
{
    private Author() : base(UserId.New())
    {
    }
    
    public string UserName { get; }
    public string? DisplayName { get; }
    public string? Email { get; }
    public DateTime MemberSince { get; }
}