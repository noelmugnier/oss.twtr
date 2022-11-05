using OSS.Twtr.App.Domain.ValueObjects;
using OSS.Twtr.Domain;

namespace OSS.Twtr.App.Domain.Entities;

public class Block : Aggregate
{
    public UserId UserId { get; }
    public UserId UserIdToBlock { get; }
    public DateTimeOffset BlockedOn { get; } = DateTimeOffset.UtcNow;

    private Block(UserId userId, UserId userIdToBlock)
    {
        UserId = userId;
        UserIdToBlock = userIdToBlock;
    }

    public static Block Create(UserId userId, UserId userIdToBlock)
    {
        return new Block(userId, userIdToBlock);
    }
}