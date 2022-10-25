using OSS.Twtr.Core;
using OSS.Twtr.Infrastructure;
using OSS.Twtr.Management.Application;
using OSS.Twtr.Domain.Repositories;
using OSS.Twtr.Management.Infrastructure.Repositories;

namespace OSS.Twtr.Management.Infrastructure.Persistence;

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