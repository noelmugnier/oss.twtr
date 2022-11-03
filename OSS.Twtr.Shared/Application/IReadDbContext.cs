namespace OSS.Twtr.Application;

public interface IReadDbContext
{
    IQueryable<T> Get<T>() where T : class;
}