using OSS.Twtr.App.Domain.Events;
using OSS.Twtr.App.Domain.ValueObjects;
using OSS.Twtr.Domain;

namespace OSS.Twtr.App.Domain.Entities;

public class Subscription : Aggregate
{
    private Subscription(){}
    
    private Subscription(UserId followerUserId, UserId subscribedToUserId)
    {
        FollowerUserId = followerUserId;
        SubscribedToUserId = subscribedToUserId;
        
        RaiseEvent(new SubscriptionAdded(subscribedToUserId.Value, followerUserId.Value));
        RaiseEvent(new FollowerAdded(subscribedToUserId.Value, followerUserId.Value));
    }

    public static Subscription CreateSubscription(UserId fromUserId, UserId toUserId)
    {
        return new Subscription(fromUserId, toUserId);
    }

    public UserId FollowerUserId { get; }
    public UserId SubscribedToUserId { get; }
    public DateTimeOffset SubscribedOn { get; } = DateTimeOffset.UtcNow;
}