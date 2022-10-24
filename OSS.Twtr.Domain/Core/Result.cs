using System.Linq.Expressions;

namespace OSS.Twtr.Domain;

public record Result<TLeft> : Either<TLeft, IEnumerable<Error>>
{
    public Result(TLeft left) : base(left)
    {
    }

    public Result(IEnumerable<Error> errors) : base(errors)
    {
    }
}

public record struct Unit()
{
}

public interface ISpecification<T> where T : notnull
{
    bool ApplyFilter(object Id);
} 

public interface IListSpecification<T> : ISpecification<T> where T : notnull
{
    Expression<Func<T, object>>? OrderBy { get; }
    Expression<Func<T, object>>? OrderByDesc { get; }
    public string ContinuationToken { get; }
}