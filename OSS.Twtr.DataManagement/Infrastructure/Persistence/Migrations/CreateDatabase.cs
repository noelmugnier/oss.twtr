using FluentMigrator;
using OSS.Twtr.Infrastructure;

namespace OSS.Twtr.TweetManagement.Infrastructure.Migrations;

[Migration(20221023164000)]
public class CreateDatabase : DbMigration
{
    public override void Up()
    {
        Create.Table(GetTableName<UserEntity>())
            .WithColumn(GetColumnName<UserEntity>(c => c.Id)).AsGuid().PrimaryKey()
            .WithColumn(GetColumnName<UserEntity>(c => c.DisplayName)).AsString()
            .WithColumn(GetColumnName<UserEntity>(c => c.UserName)).AsString()
            .WithColumn(GetColumnName<UserEntity>(c => c.MemberSince)).AsDateTime2();

        Create.Table(GetTableName<TweetEntity>())
            .WithColumn(GetColumnName<TweetEntity>(c => c.Id)).AsGuid().PrimaryKey()
            .WithColumn(GetColumnName<TweetEntity>(c => c.Message)).AsString()
            .WithColumn(GetColumnName<TweetEntity>(c => c.UserId)).AsGuid()
            .WithColumn(GetColumnName<TweetEntity>(c => c.PostedOn)).AsDateTime2()
            .ForeignKey(GetTableName<UserEntity>(), GetColumnName<UserEntity>(c => c.Id));
    }

    public override void Down()
    {
        Delete.Table(GetTableName<TweetEntity>());
        Delete.Table(GetTableName<UserEntity>());
    }
}