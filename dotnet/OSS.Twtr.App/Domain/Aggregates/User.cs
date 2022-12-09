using OSS.Twtr.App.Domain.Events;
using OSS.Twtr.App.Domain.ValueObjects;
using OSS.Twtr.Domain;

namespace OSS.Twtr.App.Domain.Entities;

public class User : Aggregate<UserId>
{
    private User() : base(UserId.New())
    {
    }
    
    public string UserName { get; }
    public string? DisplayName { get; set; }
    public string? Job { get; set; }
    public string? Description { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? Location { get; set; }
    public string? Url { get; set; }
    public DateTime MemberSince { get; }
    public TweetId? PinnedTweetId { get; private set; }
    
    public void PinTweet(TweetId tweetId)
    {
        PinnedTweetId = tweetId;
        RaiseEvent(new TweetPinned(tweetId.Value, Id.Value));
    }

    public override void Remove()
    {
    }

    public void UnpinTweet(TweetId tweetId)
    {
        if(PinnedTweetId == null) 
            return;
        
        RaiseEvent(new TweetUnpinned(PinnedTweetId.Value, Id.Value));
        PinnedTweetId = null;
    }
}