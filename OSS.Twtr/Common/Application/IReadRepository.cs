namespace OSS.Twtr.Common.Application;

public interface IReadRepository
{
    IQueryable<T> Get<T>() where T : class;
}