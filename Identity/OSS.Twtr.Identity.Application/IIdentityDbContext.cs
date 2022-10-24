using OSS.Twtr.Application;
using OSS.Twtr.Identity.Domain.Repositories;

namespace OSS.Twtr.Identity.Application;

public interface IIdentityDbContext : IUnitOfWork
{
    public IAccountRepository Accounts { get; }
}