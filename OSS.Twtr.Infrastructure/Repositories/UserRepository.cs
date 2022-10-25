using LinqToDB;
using OSS.Twtr.Core;
using OSS.Twtr.Infrastructure;
using OSS.Twtr.Domain;
using OSS.Twtr.Domain.Contracts;
using OSS.Twtr.Domain.Repositories;
using OSS.Twtr.Management.Infrastructure.Persistence;

namespace OSS.Twtr.Management.Infrastructure.Repositories;

public class UserRepository : Repository<DataDbConnection, UserEntity>, IUserRepository
{
    public UserRepository(UnitOfWork<DataDbConnection> connection)
        : base(connection)
    {
    }

    public Task<User> Get(UserId id, CancellationToken token)
    {
        var query =
            from t in Table
            where t.Id == id.Value
            select new User(UserId.From(t.Id), t.UserName, t.MemberSince, t.DisplayName);

        return query.SingleAsync(token);
    }

    public async Task<UserProfileDto> GetUserProfile(UserId id, CancellationToken token)
    {
        var query =
            from u in GetTable<UserEntity>()
            join t in GetTable<TweetEntity>() on u.Id equals t.UserId into te
            where u.Id == id.Value
            select new UserProfileDto(u.Id, u.UserName, u.DisplayName, u.MemberSince, te.Select(t => new UserTweetDto(t.Id, t.Message, t.PostedOn)));

        return await query.SingleAsync(token);
    }

    public void Add(User entity)
    {
        Execute(ct => Table.InsertAsync(() => new UserEntity
            {Id = entity.Id.Value, UserName = entity.UserName, DisplayName = entity.DisplayName, MemberSince = entity.MemberSince.DateTime}, ct), entity.DomainEvents);
    }

    public void Delete(User entity)
    {
        Execute(ct => Table
            .DeleteAsync(t => t.Id == entity.Id.Value, ct), entity.DomainEvents);
    }
}