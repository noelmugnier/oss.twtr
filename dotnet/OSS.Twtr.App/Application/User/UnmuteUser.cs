using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OSS.Twtr.App.Domain.Entities;
using OSS.Twtr.App.Domain.Repositories;
using OSS.Twtr.App.Domain.ValueObjects;
using OSS.Twtr.App.Infrastructure;
using OSS.Twtr.Application;
using OSS.Twtr.Core;

namespace OSS.Twtr.App.Application;

public record struct UnmuteUserCommand(Guid UserIdToUnmute, Guid UserId) : ICommand<Result<Unit>>;

public sealed class UnmuteUserValidator : AbstractValidator<UnmuteUserCommand>
{
    public UnmuteUserValidator()
    {
        RuleFor(x => x.UserId).NotEqual(Guid.Empty)
            .WithMessage("You must be authenticated to unmute a user");

        RuleFor(x => x.UserIdToUnmute).NotEmpty()
            .WithMessage("You must specify the user to unmute");
    }
}

internal sealed class UnmuteUserHandler : ICommandHandler<UnmuteUserCommand, Result<Unit>>
{
    private readonly AppDbContext _repository;
    public UnmuteUserHandler(AppDbContext repository) => _repository = repository;

    public async Task<Result<Unit>> Handle(UnmuteUserCommand request, CancellationToken ct)
    {
        var mute = await _repository.Set<Mute>().SingleOrDefaultAsync(b => 
            b.UserId == UserId.From(request.UserId) && b.UserIdToMute == UserId.From(request.UserIdToUnmute), ct);

        if (mute == null)
            return new Result<Unit>(Unit.Value);
        
        _repository.Remove(mute);
        
        var results= await _repository.SaveChangesAsync(ct);
        return results > 0 ? new Result<Unit>(Unit.Value):new Result<Unit>(new Error("Failed to unmute user"));
    }
}