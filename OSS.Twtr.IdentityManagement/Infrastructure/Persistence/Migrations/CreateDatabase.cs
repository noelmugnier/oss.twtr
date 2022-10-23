using FluentMigrator;
using OSS.Twtr.Infrastructure;

namespace OSS.Twtr.AccountManagement.Infrastructure.Migrations;


[Migration(20221023164000)]
public class CreateDatabase : DbMigration
{
    public override void Up()
    {
        Create.Table(GetTableName<AccountEntity>())
            .WithColumn(GetColumnName<AccountEntity>(c => c.Id)).AsGuid().PrimaryKey()
            .WithColumn(GetColumnName<AccountEntity>(c => c.Username)).AsString()
            .WithColumn(GetColumnName<AccountEntity>(c => c.PasswordHash)).AsString();
    }

    public override void Down()
    {
        Delete.Table(GetTableName<AccountEntity>());
    }
}