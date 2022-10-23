using OSS.Twtr.Domain.Ids;

namespace OSS.Twtr.AccountManagement.Domain;

public interface IAccountRepository
{
    Task<Account> Get(AccountId id, CancellationToken token);
    void Add(Account entity);
    void Update(Account entity);
    void Delete(Account entity);
}