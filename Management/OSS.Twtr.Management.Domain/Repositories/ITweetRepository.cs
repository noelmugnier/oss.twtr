using OSS.Twtr.Domain;
using OSS.Twtr.Management.Domain.Contracts;

namespace OSS.Twtr.Management.Domain.Repositories;

public interface ITweetRepository
{
    Task<TweetDto> Get(TweetId id, CancellationToken token);
    void Add(Tweet entity);
    void Update(Tweet entity);
    void Delete(Tweet entity);
}