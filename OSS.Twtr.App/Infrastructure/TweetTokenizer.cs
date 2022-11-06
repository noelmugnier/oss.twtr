using System.Text.RegularExpressions;

namespace OSS.Twtr.App.Application;

internal sealed class TweetTokenizer : ITweetTokenizer
{
    public IEnumerable<string> TokenizeMessage(string tweetMessage)
    {
        var tokens = TokenizeRegexMatches(@"(\B@\w+)", tweetMessage);
        tokens.AddRange(TokenizeRegexMatches(@"(\B#\w+)", tweetMessage));

        foreach (var token in tokens)
            tweetMessage = tweetMessage.Replace(token, string.Empty);

        tokens.AddRange(TokenizeMessageRemainingWords(tweetMessage));
        return tokens;
    }

    private IEnumerable<string> TokenizeMessageRemainingWords(string tweetMessage)
    {
        var words = tweetMessage.Split(' ').SelectMany(t => t.Split('\''));
        var keywords = string.Empty;
        var keywordsCount = 0;
        var tokens = new List<string>();
        foreach (var token in words)
        {
            if (StopWords.FR.Contains(token) || keywordsCount > 1)
            {
                if (!string.IsNullOrWhiteSpace(keywords))
                    tokens.Add(keywords);

                keywords = string.Empty;
                keywordsCount = 0;
            }
            else
            {
                keywords += keywords.Length > 0 ? " " + token : token;
                keywordsCount++;
            }
        }

        return tokens;
    }

    private List<string> TokenizeRegexMatches(string pattern, string message)
    {
        var regex = new Regex(pattern, RegexOptions.Compiled);
        var matches = regex.Match(message);
        return matches.Success
            ? matches.Groups[0].Captures.Select(c => c.Value).ToList()
            : new List<string>();
    }
}