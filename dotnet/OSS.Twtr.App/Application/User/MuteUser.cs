using FluentValidation;
using OSS.Twtr.App.Domain.Entities;
using OSS.Twtr.App.Domain.Repositories;
using OSS.Twtr.App.Domain.ValueObjects;
using OSS.Twtr.App.Infrastructure;
using OSS.Twtr.Application;
using OSS.Twtr.Core;

namespace OSS.Twtr.App.Application;

public record struct MuteUserCommand(Guid UserIdToMute, Guid UserId) : ICommand<Result<Unit>>;

public sealed class MuteUserValidator : AbstractValidator<MuteUserCommand>
{
    public MuteUserValidator()
    {
        RuleFor(x => x.UserId).NotEqual(Guid.Empty)
            .WithMessage("You must be authenticated to mute a user");

        RuleFor(x => x.UserIdToMute).NotEmpty()
            .WithMessage("You must specify the user to mute");
    }
}

internal sealed class MuteUserHandler : ICommandHandler<MuteUserCommand, Result<Unit>>
{
    private readonly AppDbContext _repository;
    public MuteUserHandler(AppDbContext repository) => _repository = repository;

    public async Task<Result<Unit>> Handle(MuteUserCommand request, CancellationToken ct)
    {
        var subscription = Mute.Create(
            UserId.From(request.UserId), 
            UserId.From(request.UserIdToMute));
        
        await _repository.AddAsync(subscription, ct);
        
        var results= await _repository.SaveChangesAsync(ct);
        return results > 0 ? new Result<Unit>(Unit.Value):new Result<Unit>(new Error("Failed to mute user"));
    }
}