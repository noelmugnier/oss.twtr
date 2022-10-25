using OSS.Twtr.Application;
using OSS.Twtr.Domain.Repositories;

namespace OSS.Twtr.Management.Application;

public interface IDataDbContext : IUnitOfWork
{
    ITweetRepository Tweets { get; }
    IUserRepository Users { get; }
}