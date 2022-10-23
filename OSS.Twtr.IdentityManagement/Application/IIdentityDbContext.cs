using OSS.Twtr.AccountManagement.Domain;
using OSS.Twtr.Application;

namespace OSS.Twtr.AccountManagement.Application;

public interface IIdentityDbContext : IUnitOfWork
{
    public IAccountRepository Accounts { get; }
}