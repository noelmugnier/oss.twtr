using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OSS.Twtr.App.Domain.Entities;
using OSS.Twtr.App.Domain.Repositories;
using OSS.Twtr.App.Domain.ValueObjects;
using OSS.Twtr.App.Infrastructure;
using OSS.Twtr.Application;
using OSS.Twtr.Core;

namespace OSS.Twtr.App.Application;

public record struct UnblockUserCommand(Guid UserIdToUnblock, Guid UserId) : ICommand<Result<Unit>>;

public sealed class UnblockUserValidator : AbstractValidator<UnblockUserCommand>
{
    public UnblockUserValidator()
    {
        RuleFor(x => x.UserId).NotEqual(Guid.Empty)
            .WithMessage("You must be authenticated to unblock a user");

        RuleFor(x => x.UserIdToUnblock).NotEmpty()
            .WithMessage("You must specify the user to unblock");
    }
}

internal sealed class UnblockUserHandler : ICommandHandler<UnblockUserCommand, Result<Unit>>
{
    private readonly AppDbContext _repository;
    public UnblockUserHandler(AppDbContext repository) => _repository = repository;

    public async Task<Result<Unit>> Handle(UnblockUserCommand request, CancellationToken ct)
    {
        var block = await _repository.Set<Block>().SingleOrDefaultAsync(b => 
            b.UserId == UserId.From(request.UserId) && b.UserIdToBlock == UserId.From(request.UserIdToUnblock), ct);

        if (block == null)
            return new Result<Unit>(Unit.Value);
        
        _repository.Remove(block);
        
        var results= await _repository.SaveChangesAsync(ct);
        return results > 0 ? new Result<Unit>(Unit.Value):new Result<Unit>(new Error("Failed to unblock user"));
    }
}