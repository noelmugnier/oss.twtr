namespace OSS.Twtr.Core;


public abstract class Specification<T> : Specification<T, T> where T : notnull
{
}

public abstract class Specification<T, TU> where T : notnull
{
    public bool IsSatisfiedBy(T item)
    {
        return SatisfyingElementsFrom(new[] {item}.AsQueryable()).Any();
    }

    public abstract IQueryable<TU> SatisfyingElementsFrom(IQueryable<T> candidates);
}