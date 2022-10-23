using LinqToDB;
using LinqToDB.Configuration;
using LinqToDB.Data;

namespace OSS.Twtr.TweetManagement.Infrastructure;

public class DataDbConnection : DataConnection
{
    public DataDbConnection(LinqToDBConnectionOptions<DataDbConnection> options)
        : base(options)
    {
    }
    
    public DataDbConnection(string connectionString)
        : base(new LinqToDBConnectionOptions<DataDbConnection>(new LinqToDBConnectionOptionsBuilder().UseSQLite(connectionString)))
    {
        this.CreateTable<TweetEntity>();
        this.CreateTable<UserEntity>();
    }

    public ITable<TweetEntity> Tweets => this.GetTable<TweetEntity>();
    public ITable<UserEntity> Users => this.GetTable<UserEntity>();
}