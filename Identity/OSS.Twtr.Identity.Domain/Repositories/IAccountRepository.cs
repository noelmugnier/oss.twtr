using OSS.Twtr.Domain;

namespace OSS.Twtr.Identity.Domain.Repositories;

public interface IAccountRepository
{
    Task<Account> Get(AccountId id, CancellationToken token);
    void Add(Account entity);
    void Update(Account entity);
    void Delete(Account entity);
}