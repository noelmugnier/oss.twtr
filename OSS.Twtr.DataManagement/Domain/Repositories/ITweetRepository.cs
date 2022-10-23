using OSS.Twtr.Domain.Ids;

namespace OSS.Twtr.TweetManagement.Domain;

public interface ITweetRepository
{
    Task<TweetDto> Get(TweetId id, CancellationToken token);
    void Add(Tweet entity);
    void Update(Tweet entity);
    void Delete(Tweet entity);
}