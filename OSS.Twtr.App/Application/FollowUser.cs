using FluentValidation;
using OSS.Twtr.App.Domain.Entities;
using OSS.Twtr.App.Domain.Repositories;
using OSS.Twtr.App.Domain.ValueObjects;
using OSS.Twtr.App.Infrastructure;
using OSS.Twtr.Application;
using OSS.Twtr.Core;

namespace OSS.Twtr.App.Application;

public record struct FollowUserCommand(Guid UserIdToFollow, Guid UserId) : ICommand<Result<Unit>>;

public sealed class FollowUserValidator : AbstractValidator<FollowUserCommand>
{
    public FollowUserValidator()
    {
        RuleFor(x => x.UserId).NotEqual(Guid.Empty)
            .WithMessage("You must be authenticated to like a tweet");

        RuleFor(x => x.UserIdToFollow).NotEmpty()
            .WithMessage("You must specify the user to follow");
    }
}

internal sealed class FollowUserHandler : ICommandHandler<FollowUserCommand, Result<Unit>>
{
    private readonly AppDbContext _repository;
    public FollowUserHandler(AppDbContext repository) => _repository = repository;

    public async Task<Result<Unit>> Handle(FollowUserCommand request, CancellationToken ct)
    {
        var subscription = Subscription.CreateSubscription(
            UserId.From(request.UserId), 
            UserId.From(request.UserIdToFollow));
        
        await _repository.AddAsync(subscription, ct);
        
        var results= await _repository.SaveChangesAsync(ct);
        return results > 0 ? new Result<Unit>(Unit.Value):new Result<Unit>(new Error("Failed to follow user"));
    }
}