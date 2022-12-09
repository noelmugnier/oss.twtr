namespace OSS.Twtr.App.Domain.Entities;

public class Trending
{
    public Trending(string name, int tweetCount, DateTime analyzedOn)
    {
        Name = name;
        TweetCount = tweetCount;
        AnalyzedOn = analyzedOn;
    }

    public DateTime AnalyzedOn { get; }
    public string Name { get; }
    public int TweetCount { get; }
}