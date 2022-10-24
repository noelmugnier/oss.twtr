using OSS.Twtr.Domain;
using OSS.Twtr.Identity.Application;
using OSS.Twtr.Identity.Domain.Repositories;
using OSS.Twtr.Identity.Infrastructure.Repositories;
using OSS.Twtr.Infrastructure;

namespace OSS.Twtr.Identity.Infrastructure.Persistence;

public class IdentityDbContext : UnitOfWork<IdentityDbConnection>, IIdentityDbContext
{
    public IdentityDbContext(IdentityDbConnection connection, IEventDispatcher dispatcher) 
        : base(connection, dispatcher)
    {
        Accounts = new AccountRepository(this);
    }
    
    public IAccountRepository Accounts { get; }
}