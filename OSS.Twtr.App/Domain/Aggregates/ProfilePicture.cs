using OSS.Twtr.App.Domain.ValueObjects;

namespace OSS.Twtr.App.Domain.Entities;

public class ProfilePicture
{
    public UserId UserId { get; }
    public byte[] Data { get; }

    private ProfilePicture(){}
    
    private ProfilePicture(UserId userId, byte[] data)
    {
        UserId = userId;
        Data = data;
    }

    public static ProfilePicture Create(UserId userId, byte[] data)
    {
        return new ProfilePicture(userId, data);
    }
}