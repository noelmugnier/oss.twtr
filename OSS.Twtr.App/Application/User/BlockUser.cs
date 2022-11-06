using FluentValidation;
using OSS.Twtr.App.Domain.Entities;
using OSS.Twtr.App.Domain.Repositories;
using OSS.Twtr.App.Domain.ValueObjects;
using OSS.Twtr.App.Infrastructure;
using OSS.Twtr.Application;
using OSS.Twtr.Core;

namespace OSS.Twtr.App.Application;

public record struct BlockUserCommand(Guid UserIdToBlock, Guid UserId) : ICommand<Result<Unit>>;

public sealed class BlockUserValidator : AbstractValidator<BlockUserCommand>
{
    public BlockUserValidator()
    {
        RuleFor(x => x.UserId).NotEqual(Guid.Empty)
            .WithMessage("You must be authenticated to block a user");

        RuleFor(x => x.UserIdToBlock).NotEmpty()
            .WithMessage("You must specify the user to block");
    }
}

internal sealed class BlockUserHandler : ICommandHandler<BlockUserCommand, Result<Unit>>
{
    private readonly AppDbContext _repository;
    public BlockUserHandler(AppDbContext repository) => _repository = repository;

    public async Task<Result<Unit>> Handle(BlockUserCommand request, CancellationToken ct)
    {
        var blockedUser = Block.Create(
            UserId.From(request.UserId), 
            UserId.From(request.UserIdToBlock));
        
        await _repository.AddAsync(blockedUser, ct);
        
        var results= await _repository.SaveChangesAsync(ct);
        return results > 0 ? new Result<Unit>(Unit.Value):new Result<Unit>(new Error("Failed to Block user"));
    }
}