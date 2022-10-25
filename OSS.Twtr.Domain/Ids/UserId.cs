namespace OSS.Twtr.Core;

public record struct UserId()
{
    public Guid Value { get; private init; } = Guid.NewGuid();
    public static UserId From(Guid value) => new (){Value = value};
    public static UserId New() =>  new (){Value = Guid.NewGuid()};
}