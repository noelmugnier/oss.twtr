using FluentValidation;
using LinqToDB;
using LinqToDB.EntityFrameworkCore;
using OSS.Twtr.App.Domain.Entities;
using OSS.Twtr.App.Domain.Repositories;
using OSS.Twtr.App.Domain.ValueObjects;
using OSS.Twtr.App.Infrastructure;
using OSS.Twtr.Application;
using OSS.Twtr.Core;

namespace OSS.Twtr.App.Application;

public record struct GetUserQuery(Guid UserIdToRetrieve, Guid? UserId) : IQuery<Result<UserDto>>;
public record struct UserDto(Guid Id, string UserName, string DisplayName, DateTime MemberSince, string? Job, string? 
Description, string? Url, DateTime? BirthDate, string? Location, int FollowingCount, int FollowedByCount, bool Followed, bool Blocked, bool Muted);

public sealed class GetUserValidator : AbstractValidator<GetUserQuery>
{
    public GetUserValidator()
    {
        RuleFor(x => x.UserIdToRetrieve).NotEmpty()
            .WithMessage("You must specify the user to retrieve");
    }
}

internal sealed class GetUserHandler : IQueryHandler<GetUserQuery, Result<UserDto>>
{
    private readonly ReadDbContext _repository;
    public GetUserHandler(ReadDbContext repository) => _repository = repository;

    public async Task<Result<UserDto>> Handle(GetUserQuery request, CancellationToken ct)
    {
        IQueryable<UserDto> userQuery;
        
        if(request.UserId.HasValue)
            userQuery = from author in _repository.Set<ReadOnlyUser>()
                from followed in _repository.Set<ReadOnlySubscription>()
                    .LeftJoin(s => s.FollowerUserId == request.UserId && s.SubscribedToUserId == author.Id)
                from blocked in _repository.Set<ReadOnlyBlock>()
                    .LeftJoin(s => s.UserId == request.UserId && s.UserIdToBlock == author.Id)
                from muted in _repository.Set<ReadOnlyMute>()
                    .LeftJoin(s => s.UserId == request.UserId && s.UserIdToMute == author.Id)
                from subscription in _repository.Set<ReadOnlySubscription>()
                    .LeftJoin(s => s.FollowerUserId == author.Id || s.SubscribedToUserId == author.Id)
                where author.Id == request.UserIdToRetrieve
                group new { subscription, followed, blocked, muted} by author into g
                select new UserDto(g.Key.Id, g.Key.UserName, g.Key.DisplayName, g.Key.MemberSince, g.Key.Job, 
                    g.Key.Description, g.Key.Url, g.Key.BirthDate, g.Key.Location,
                    g.Count(s => s.subscription.FollowerUserId == g.Key.Id), 
                    g.Count(s => s.subscription.SubscribedToUserId == g.Key.Id), 
                    g.Count(c => c.followed != null) > 0, 
                    g.Count(s => s.blocked != null) > 0, 
                    g.Count(s => s.muted != null) > 0);
        else
            userQuery = from author in _repository.Set<ReadOnlyUser>()
                from subscription in _repository.Set<ReadOnlySubscription>()
                    .LeftJoin(s => s.FollowerUserId == author.Id || s.SubscribedToUserId == author.Id)
                where author.Id == request.UserIdToRetrieve
                group subscription by author
                into g
                select new UserDto(g.Key.Id, g.Key.UserName, g.Key.DisplayName, g.Key.MemberSince, g.Key.Job, 
                    g.Key.Description, g.Key.Url, g.Key.BirthDate, g.Key.Location,
                    g.Count(s => s.FollowerUserId == g.Key.Id), 
                    g.Count(s => s.SubscribedToUserId == g.Key.Id), 
                    false, false, false);

        var user = await userQuery.SingleOrDefaultAsyncLinqToDB(ct);
        Console.WriteLine(user.Id);
        return new Result<UserDto>(user);
    }
}