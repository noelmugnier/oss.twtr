namespace OSS.Twtr.TweetManagement.Domain;

public class UserTweetDto
{
    public UserTweetDto(Guid id, string message, DateTimeOffset postedOn)
    {
        Id = id;
        Message = message;
        PostedOn = postedOn;
    }

    public Guid Id { get; }
    public string Message { get; }
    public DateTimeOffset PostedOn { get; }
}