﻿using LinqToDB;
using OSS.Twtr.Domain;
using OSS.Twtr.Infrastructure;
using OSS.Twtr.Management.Domain;
using OSS.Twtr.Management.Domain.Contracts;
using OSS.Twtr.Management.Domain.Repositories;
using OSS.Twtr.Management.Infrastructure.Persistence;

namespace OSS.Twtr.Management.Infrastructure.Repositories;

public class TweetRepository : Repository<DataDbConnection, TweetEntity>, ITweetRepository
{
    public TweetRepository(UnitOfWork<DataDbConnection> connection)
        : base(connection)
    {
    }

    public Task<T> Get<T>(ISpecification<TweetDto, T> specification, CancellationToken token)
    {
        var query =
            from t in Table
            join u in GetTable<UserEntity>() on t.UserId equals u.Id
            select new TweetDto(t.Id, t.Message, t.PostedOn, new UserDto(u.Id, u.UserName, u.DisplayName));

        var result = specification.SatisfyingElementsFrom(query);
        return result.SingleAsync(token);
    }

    public async Task<IEnumerable<T>> Get<T>(IListSpecification<TweetDto, T> specification, CancellationToken token)
    {
        var query =
            from t in Table
            join u in GetTable<UserEntity>() on t.UserId equals u.Id 
            select new TweetDto(t.Id, t.Message, t.PostedOn, new UserDto(u.Id, u.UserName, u.DisplayName));

        var result = specification.SatisfyingElementsFrom(query);
        return await result.ToListAsync(token);
    }

    public void Add(Tweet entity)
    {
        Execute(ct => Table.InsertAsync(() => new TweetEntity
            {Id = entity.Id.Value, Message = entity.Message, UserId = entity.User.Id.Value, PostedOn = entity.PostedOn.DateTime }, ct), entity.DomainEvents);
    }

    public void Update(Tweet entity)
    {
        Execute(ct => Table
            .Where(t => t.Id == entity.Id.Value)
            .Set(t => t.Message, entity.Message)
            .UpdateAsync(ct), entity.DomainEvents);
    }

    public void Delete(Tweet entity)
    {
        Execute(ct => Table
            .DeleteAsync(t => t.Id == entity.Id.Value, ct), entity.DomainEvents);
    }
}