using OSS.Twtr.AccountManagement.Application;
using OSS.Twtr.AccountManagement.Domain;
using OSS.Twtr.Domain.Services;
using OSS.Twtr.Infrastructure;

namespace OSS.Twtr.AccountManagement.Infrastructure;

public class IdentityDbContext : UnitOfWork<IdentityDbConnection>, IIdentityDbContext
{
    public IdentityDbContext(IdentityDbConnection connection, IEventDispatcher dispatcher) 
        : base(connection, dispatcher)
    {
        Accounts = new AccountRepository(this);
    }
    
    public IAccountRepository Accounts { get; }
}