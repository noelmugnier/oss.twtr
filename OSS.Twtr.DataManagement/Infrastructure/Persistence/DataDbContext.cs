using OSS.Twtr.Domain.Services;
using OSS.Twtr.Infrastructure;
using OSS.Twtr.TweetManagement.Application;
using OSS.Twtr.TweetManagement.Domain;

namespace OSS.Twtr.TweetManagement.Infrastructure;

public class DataDbContext : UnitOfWork<DataDbConnection>, IDataDbContext
{
    public DataDbContext(DataDbConnection dbConnection, IEventDispatcher dispatcher) 
        : base(dbConnection, dispatcher)
    {
        Tweets = new TweetRepository(this);
        Users = new UserRepository(this);
    }
    
    public ITweetRepository Tweets { get; }
    public IUserRepository Users { get; }
}