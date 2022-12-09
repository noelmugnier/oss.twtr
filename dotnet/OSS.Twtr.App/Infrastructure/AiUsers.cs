using OSS.Twtr.App.Domain.ValueObjects;

namespace OSS.Twtr.App.Application;

internal class AiUsers
{
    public static UserId Positive => UserId.From(Guid.Parse("26B1C77F-12E2-4B94-AA6C-E6FFF86E09F3"));
    public static UserId Neutral => UserId.From(Guid.Parse("D3910D3E-6421-407C-9244-706A550545E5"));
    public static UserId Negative => UserId.From(Guid.Parse("E9D62AA9-E3CE-4D8D-961D-3095F321ABCF"));
}