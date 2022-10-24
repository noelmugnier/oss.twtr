namespace OSS.Twtr.Management.Domain.Contracts;

public record TweetDto(Guid Id, string Message, DateTimeOffset PostedOn, UserDto User)
{
    public Guid UserId { get; } = User.Id;
}