namespace OSS.Twtr.TweetManagement.Domain;

public class TweetDto
{
    public TweetDto(Guid id, string message, DateTimeOffset postedOn, string userName, string displayName)
    {
        Id = id;
        Message = message;
        PostedOn = postedOn;
        UserName = userName;
        DisplayName = displayName;
    }

    public Guid Id { get; }
    public string Message { get; }
    public string UserName { get; }
    public string DisplayName { get; }
    public DateTimeOffset PostedOn { get; }
}