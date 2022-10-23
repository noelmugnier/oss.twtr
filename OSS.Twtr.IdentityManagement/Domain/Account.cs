using OSS.Twtr.Domain;
using OSS.Twtr.Domain.Events;
using OSS.Twtr.Domain.Ids;

namespace OSS.Twtr.AccountManagement.Domain;

public class Account : Entity
{
    public Account(string userName, string passwordHash)
        : this(AccountId.New(), userName, passwordHash)
    {
        RaiseEvent(new AccountCreated(Id, userName));
    }

    public Account(AccountId id, string userName, string passwordHash)
    {
        Id = id;
        UserName = userName;
        PasswordHash = passwordHash;
    }

    public AccountId Id { get; }
    public string UserName { get; }
    public string PasswordHash { get; }
}
