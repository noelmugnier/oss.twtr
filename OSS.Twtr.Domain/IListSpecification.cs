namespace OSS.Twtr.Domain;

public interface IListSpecification<T, TU> : ISpecification<T, TU> where T : notnull
{
    public int Page { get; }
    public int ItemsPerPage { get; }
}

public interface IListSpecification<T> : IListSpecification<T, T> where T : notnull
{
}

public abstract class ListSpecification<T> : ListSpecification<T, T> where T : notnull
{
    protected ListSpecification(int page, int itemsPerPage) : base(page, itemsPerPage)
    {
    }
}

public abstract class ListSpecification<T, TU> : Specification<T, TU>, IListSpecification<T, TU> where T : notnull
{
    protected ListSpecification(int page, int itemsPerPage)
    {
        Page = page < 1 ? 1 : page;
        ItemsPerPage = itemsPerPage < 10 ? 10 : itemsPerPage;
    }
    
    public int Page { get; }
    public int ItemsPerPage { get; }
}