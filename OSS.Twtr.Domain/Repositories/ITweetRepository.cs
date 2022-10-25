using OSS.Twtr.Core;
using OSS.Twtr.Domain.Contracts;

namespace OSS.Twtr.Domain.Repositories;

public interface ITweetRepository
{
    Task<T> Get<T>(Specification<TweetDto, T> id, CancellationToken token);
    Task<IEnumerable<T>> GetAll<T>(Specification<TweetDto, T> id, CancellationToken token);
    void Add(Tweet entity);
    void Delete(Tweet entity);
}