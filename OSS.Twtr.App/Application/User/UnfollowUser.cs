using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OSS.Twtr.App.Domain.Entities;
using OSS.Twtr.App.Domain.Repositories;
using OSS.Twtr.App.Domain.ValueObjects;
using OSS.Twtr.App.Infrastructure;
using OSS.Twtr.Application;
using OSS.Twtr.Core;

namespace OSS.Twtr.App.Application;

public record struct UnfollowUserCommand(Guid UserIdToUnsubscribe, Guid UserId) : ICommand<Result<Unit>>;

public sealed class UnfollowUserValidator : AbstractValidator<UnfollowUserCommand>
{
    public UnfollowUserValidator()
    {
        RuleFor(x => x.UserId).NotEqual(Guid.Empty)
            .WithMessage("You must be authenticated to unfollow a user");

        RuleFor(x => x.UserIdToUnsubscribe).NotEmpty()
            .WithMessage("You must specify the user to unfollow");
    }
}

internal sealed class UnfollowUserHandler : ICommandHandler<UnfollowUserCommand, Result<Unit>>
{
    private readonly AppDbContext _repository;
    public UnfollowUserHandler(AppDbContext repository) => _repository = repository;

    public async Task<Result<Unit>> Handle(UnfollowUserCommand request, CancellationToken ct)
    {
        var block = await _repository.Set<Subscription>().SingleOrDefaultAsync(b => 
            b.FollowerUserId == UserId.From(request.UserId) && b.SubscribedToUserId == UserId.From(request.UserIdToUnsubscribe), ct);

        if (block == null)
            return new Result<Unit>(Unit.Value);
        
        _repository.Remove(block);
        
        var results= await _repository.SaveChangesAsync(ct);
        return results > 0 ? new Result<Unit>(Unit.Value):new Result<Unit>(new Error("Failed to unfollow user"));
    }
}