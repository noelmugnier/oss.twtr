using LinqToDB;
using OSS.Twtr.AccountManagement.Domain;
using OSS.Twtr.Domain.Ids;
using OSS.Twtr.Infrastructure;

namespace OSS.Twtr.AccountManagement.Infrastructure;

public class AccountRepository : Repository<IdentityDbConnection, AccountEntity>, IAccountRepository
{
    public AccountRepository(UnitOfWork<IdentityDbConnection> database) 
        : base(database)
    {
    }

    public Task<Account> Get(AccountId id, CancellationToken token)
    {
        var query = 
            from t in Table
            where t.Id == id.Value
            select new Account(AccountId.From(t.Id), t.Username, t.PasswordHash);

        return query.SingleAsync(token);
    }

    public void Add(Account entity)
    {
        Execute(ct => Table.InsertAsync(() => new AccountEntity()
            {Id = entity.Id.Value, Username = entity.UserName, PasswordHash = entity.PasswordHash}, ct), entity.DomainEvents);
    }

    public void Update(Account entity)
    {
        Execute(ct => Table
            .Where(t => t.Id == entity.Id.Value)
            .Set(t => t.PasswordHash, entity.PasswordHash)
            .UpdateAsync(ct), entity.DomainEvents);
    }

    public void Delete(Account entity)
    {
        Execute(ct => Table
            .DeleteAsync(t => t.Id == entity.Id.Value, ct), entity.DomainEvents);
    }
}