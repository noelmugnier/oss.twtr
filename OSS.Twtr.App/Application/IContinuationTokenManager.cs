namespace OSS.Twtr.App.Application;

public interface IContinuationTokenManager
{
    string CreateContinuationToken(DateTime now, int skip);
    (DateTime now, int skip) ReadContinuationToken(string? continuationToken);
}