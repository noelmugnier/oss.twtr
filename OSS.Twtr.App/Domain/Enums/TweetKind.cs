namespace OSS.Twtr.App.Domain.Enums;

public enum TweetKind
{
    Tweet,
    Retweet,
    Quote,
    Reply,
}

public enum TweetAllowedReplies
{
    All,
    Following,
    Mentioned
}