namespace OSS.Twtr.Application;

public interface IReadRepository
{
    IQueryable<T> Get<T>() where T : class;
}