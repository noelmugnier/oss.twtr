using OSS.Twtr.Application;
using OSS.Twtr.TweetManagement.Domain;

namespace OSS.Twtr.TweetManagement.Application;

public interface IDataDbContext : IUnitOfWork
{
    ITweetRepository Tweets { get; }
    IUserRepository Users { get; }
}