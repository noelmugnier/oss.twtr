namespace OSS.Twtr.Domain;


public interface ISpecification<T> : ISpecification<T, T> where T : notnull
{
}

public interface ISpecification<T, TU> where T : notnull
{
    bool IsSatisfiedBy(T item);
    IQueryable<TU> SatisfyingElementsFrom(IQueryable<T> queryable);
}

public abstract class Specification<T> : Specification<T, T> where T : notnull
{
}

public abstract class Specification<T, TU> : ISpecification<T, TU> where T : notnull
{
    public bool IsSatisfiedBy(T item)
    {
        return SatisfyingElementsFrom(new[] {item}.AsQueryable()).Any();
    }

    public abstract IQueryable<TU> SatisfyingElementsFrom(IQueryable<T> candidates);
}