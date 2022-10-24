namespace OSS.Twtr.Domain;

public record struct RoleId()
{
    public Guid Value { get; private init; } = Guid.NewGuid();
    public static RoleId New() =>  new (){Value = Guid.NewGuid()};
    public static RoleId From(Guid value) => new (){Value = value};
}