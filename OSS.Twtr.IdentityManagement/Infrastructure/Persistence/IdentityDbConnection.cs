using LinqToDB;
using LinqToDB.Configuration;
using LinqToDB.Data;

namespace OSS.Twtr.AccountManagement.Infrastructure;

public class IdentityDbConnection : DataConnection
{
    public IdentityDbConnection(LinqToDBConnectionOptions<IdentityDbConnection> options)
        : base(options)
    {
    }
    
    public IdentityDbConnection(string connectionString)
        : base(new LinqToDBConnectionOptions<IdentityDbConnection>(new LinqToDBConnectionOptionsBuilder().UseSQLite(connectionString)))
    {
        this.CreateTable<AccountEntity>();
    }

    public ITable<AccountEntity> Accounts => this.GetTable<AccountEntity>();
}