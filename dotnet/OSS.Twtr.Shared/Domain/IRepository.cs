using OSS.Twtr.Core;

namespace OSS.Twtr.Domain;

public interface IRepository<T, in TId>
    where T : IAggregate
    where TId : IdentifiableId
{
    Task<Result<T>> Get(TId id, CancellationToken token);
    Task<Result<bool>> Exists(TId id, CancellationToken token);
    Task<Result<Unit>> Save(CancellationToken token);
    Task<Result<Unit>> Remove(TId entity, CancellationToken token);
}