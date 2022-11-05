using OSS.Twtr.App.Domain.ValueObjects;
using OSS.Twtr.Domain;

namespace OSS.Twtr.App.Domain.Entities;

public class Mute : Aggregate
{
    public UserId UserId { get; }
    public UserId UserIdToMute { get; }
    public DateTimeOffset MutedOn { get; } = DateTimeOffset.UtcNow;

    private Mute(UserId userId, UserId userIdToMute)
    {
        UserId = userId;
        UserIdToMute = userIdToMute;
    }

    public static Mute Create(UserId userId, UserId userIdToMute)
    {
        return new Mute(userId, userIdToMute);
    }
}