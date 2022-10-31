using OSS.Twtr.Common.Core;

namespace OSS.Twtr.Common.Domain;

public interface IRepository<T, in TId>
    where T : AggregateRoot<TId>
    where TId : IdentifiableId
{
    Task<Result<T>> Get(TId id, CancellationToken token);
    Task<Result<Unit>> Save(T entity, CancellationToken token);
}