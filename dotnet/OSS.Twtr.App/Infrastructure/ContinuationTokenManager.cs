using System.Text;

namespace OSS.Twtr.App.Application;


public class ContinuationTokenManager : IContinuationTokenManager
{
    public string? CreateContinuationToken(DateTime now, int skip, bool hasMore)
    {
        return !hasMore ? null : Convert.ToBase64String(Encoding.UTF8.GetBytes($"{now:s}_{skip}"));
    }

    public (DateTime now, int skip) ReadContinuationToken(string? continuationToken)
    {
        DateTime now = DateTime.Now;
        int skip = 0;

        if (string.IsNullOrWhiteSpace(continuationToken))
            return (now, skip);
        
        var tokens = Encoding.UTF8.GetString(Convert.FromBase64String(continuationToken)).Split("_");
        now = DateTime.Parse(tokens[0]);
        skip = int.Parse(tokens[1]);
        return (now, skip);
    }
}