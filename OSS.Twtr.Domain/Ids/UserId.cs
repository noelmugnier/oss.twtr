namespace OSS.Twtr.Core;

public record struct UserId()
{
    public Guid Value { get; private init; } = Guid.NewGuid();
    public static UserId From(Guid value) => new (){Value = value};
    public static UserId New() =>  new (){Value = Guid.NewGuid()};
    public static implicit operator Guid(UserId d) => d.Value;
    public static explicit operator UserId(Guid b) => From(b);
    public static implicit operator string(UserId d) => d.Value.ToString("D");
    public static explicit operator UserId(string b) => From(Guid.Parse(b));
}