using LinqToDB;
using LinqToDB.Data;
using OSS.Twtr.Domain;

namespace OSS.Twtr.Infrastructure;

public abstract class Repository<T>
    where T : DataConnection
{
    private readonly UnitOfWork<T> _unitOfWork;

    protected Repository(UnitOfWork<T> unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    protected ITable<TU> GetTable<TU>() where TU : class
    {
        return _unitOfWork.GetTable<TU>();
    }

    protected void Execute(Func<CancellationToken, Task<int>> command, IEnumerable<DomainEvent> events)
    {
        _unitOfWork.Execute(command, events);
    }
}

public abstract class Repository<T, TU> : Repository<T>
    where T : DataConnection
    where TU : class
{
    protected readonly ITable<TU> Table;

    protected Repository(UnitOfWork<T> unitOfWork)
        : base(unitOfWork)
    {
        Table = GetTable<TU>();
    }
}