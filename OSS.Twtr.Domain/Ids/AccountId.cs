namespace OSS.Twtr.Domain.Ids;

public record struct AccountId()
{
    public Guid Value { get; private init; } = Guid.NewGuid();
    public static AccountId New() =>  new (){Value = Guid.NewGuid()};
    public static AccountId From(Guid value) => new (){Value = value};
}