namespace OSS.Twtr.App.Application;

public interface IContinuationTokenManager
{
    string? CreateContinuationToken(DateTime now, int skip, bool hasMore);
    (DateTime now, int skip) ReadContinuationToken(string? continuationToken);
}