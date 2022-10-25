using OSS.Twtr.Core;

namespace OSS.Twtr.Application;

public interface IUnitOfWork
{
    Task<Result<Unit>> BeginTransaction(CancellationToken token);
    Task<Result<int>> Commit(CancellationToken token);
    Task<Result<Unit>> Rollback(CancellationToken token);
    Task<Result<int>> SaveChanges(CancellationToken token);
}