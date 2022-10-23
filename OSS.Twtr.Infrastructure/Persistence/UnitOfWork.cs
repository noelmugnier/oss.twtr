using LinqToDB;
using LinqToDB.Data;
using OSS.Twtr.Application;
using OSS.Twtr.Core;
using OSS.Twtr.Domain;
using OSS.Twtr.Domain.Services;

namespace OSS.Twtr.Infrastructure;

public abstract class UnitOfWork<T> : IUnitOfWork
    where T : DataConnection
{
    private readonly T _connection;
    private readonly IEventDispatcher _eventDispatcher;
    private readonly IList<KeyValuePair<Func<CancellationToken, Task<int>>, IEnumerable<DomainEvent>>> _enqueuedTasks = new List<KeyValuePair<Func<CancellationToken, Task<int>>, IEnumerable<DomainEvent>>>();
    private DataConnectionTransaction? _transaction = null;
    private readonly List<DomainEvent> _events = new();

    protected UnitOfWork(T dbConnection, IEventDispatcher eventDispatcher)
    {
        _connection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
        _eventDispatcher = eventDispatcher;
    }

    public async Task<Result<Unit>> BeginTransaction(CancellationToken token)
    {
        try
        {
            _transaction ??= await _connection.BeginTransactionAsync(token);
            return new Result<Unit>(new Unit());
        }
        catch (Exception e)
        {
            return new Result<Unit>(new List<Error> {new(e.Message)});
        }
    }

    public async Task<Result<int>> Commit(CancellationToken token)
    {
        if (_transaction == null)
            throw new InvalidOperationException("Transaction is not open");

        try
        {
            var result = await SaveChanges(token);
            await result.On(async () =>
            {
                await _transaction.CommitAsync(token);
                await DispatchEvents(token);
            }, async () =>
            {
                ClearEvents();
                await _transaction.RollbackAsync(token);
            });
            return result;
        }
        catch (Exception e)
        {
            return new Result<int>(new List<Error> {new(e.Message)});
        }
    }

    public async Task<Result<int>> SaveChanges(CancellationToken token)
    {
        var modifiedRows = 0;
        var errors = new List<Error>();
        foreach (var task in _enqueuedTasks)
        {
            try
            {
                modifiedRows += await task.Key(token);
                _events.AddRange(task.Value);
            }
            catch (Exception e)
            {
                errors.Add(new Error(e.Message, "DatabaseError"));
            }
        }

        if (_transaction == null)
            await DispatchEvents(token);

        return errors.Any() ? new Result<int>(errors) : new Result<int>(modifiedRows);
    }

    public async Task<Result<Unit>> Rollback(CancellationToken token)
    {
        if (_transaction == null)
            throw new InvalidOperationException("Transaction is not open");

        try
        {
            ClearEvents();
            await _transaction.RollbackAsync(token);
            return new Result<Unit>(new Unit());
        }
        catch (Exception e)
        {
            return new Result<Unit>(new List<Error> {new(e.Message)});
        }
    }

    internal ITable<TU> GetTable<TU>() where TU : class
    {
        return _connection.GetTable<TU>();
    }

    internal void Execute(Func<CancellationToken, Task<int>> command, IEnumerable<DomainEvent> events)
    {
        _enqueuedTasks.Add(new KeyValuePair<Func<CancellationToken, Task<int>>, IEnumerable<DomainEvent>>(command, events));
    }

    private Task DispatchEvents(CancellationToken token)
    {
        _eventDispatcher.Dispatch(_events);
        ClearEvents();
        return Task.CompletedTask;
    }

    private void ClearEvents()
    {
        _events.Clear();
    }
}