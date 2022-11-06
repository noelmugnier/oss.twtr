using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OSS.Twtr.App.Domain.Entities;
using OSS.Twtr.App.Domain.Repositories;
using OSS.Twtr.App.Domain.ValueObjects;
using OSS.Twtr.App.Infrastructure;
using OSS.Twtr.Application;
using OSS.Twtr.Core;

namespace OSS.Twtr.App.Application;

public record struct UnlikeTweetCommand(Guid UserId, Guid TweetId) : ICommand<Result<Unit>>;

public sealed class UnlikeTweetValidator : AbstractValidator<UnlikeTweetCommand>
{
    public UnlikeTweetValidator()
    {
        RuleFor(x => x.UserId).NotEqual(Guid.Empty)
            .WithMessage("You must be authenticated to unlike a Tweet");

        RuleFor(x => x.TweetId).NotEqual(Guid.Empty)
            .WithMessage("You must specify the tweet to unlike");
    }
}

internal sealed class UnlikeTweetHandler : ICommandHandler<UnlikeTweetCommand, Result<Unit>>
{
    private readonly AppDbContext _repository;
    public UnlikeTweetHandler(AppDbContext repository) => _repository = repository;

    public async Task<Result<Unit>> Handle(UnlikeTweetCommand request, CancellationToken ct)
    {
        var like = await _repository.Set<Like>().SingleOrDefaultAsync(b => 
            b.UserId == UserId.From(request.UserId) && b.TweetId == TweetId.From(request.TweetId), ct);

        if (like == null)
            return new Result<Unit>(Unit.Value);
        
        _repository.Remove(like);
        
        var results= await _repository.SaveChangesAsync(ct);
        return results > 0 ? new Result<Unit>(Unit.Value):new Result<Unit>(new Error("Failed to unlike tweet"));
    }
}