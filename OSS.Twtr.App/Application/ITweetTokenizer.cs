namespace OSS.Twtr.App.Application;

internal interface ITweetTokenizer
{
    IEnumerable<string> TokenizeMessage(string tweetMessage);
}