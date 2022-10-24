using OSS.Twtr.Domain;
using OSS.Twtr.Management.Domain.Contracts;

namespace OSS.Twtr.Management.Domain.Repositories;

public interface ITweetRepository
{
    Task<T> Get<T>(ISpecification<TweetDto, T> id, CancellationToken token);
    Task<IEnumerable<T>> Get<T>(IListSpecification<TweetDto, T> id, CancellationToken token);
    void Add(Tweet entity);
    void Delete(Tweet entity);
}