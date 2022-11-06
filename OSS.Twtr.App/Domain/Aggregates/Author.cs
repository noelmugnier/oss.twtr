using OSS.Twtr.App.Domain.Events;
using OSS.Twtr.App.Domain.ValueObjects;
using OSS.Twtr.Domain;

namespace OSS.Twtr.App.Domain.Entities;

public class Author : Aggregate<UserId>
{
    private Author() : base(UserId.New())
    {
    }
    
    public string UserName { get; }
    public string? DisplayName { get; }
    public string? Email { get; }
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