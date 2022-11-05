using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OSS.Twtr.App.Domain.Entities;
using OSS.Twtr.App.Domain.Repositories;
using OSS.Twtr.App.Domain.ValueObjects;
using OSS.Twtr.App.Infrastructure;
using OSS.Twtr.Application;
using OSS.Twtr.Core;

namespace OSS.Twtr.App.Application;

public record struct RemoveTweetCommand(Guid UserId, Guid TweetId) : ICommand<Result<Unit>>;

public sealed class RemoveTweetValidator : AbstractValidator<RemoveTweetCommand>
{
    public RemoveTweetValidator()
    {
        RuleFor(x => x.UserId).NotEqual(Guid.Empty)
            .WithMessage("You must be authenticated to remove a tweet");

        RuleFor(x => x.TweetId).NotEqual(Guid.Empty)
            .WithMessage("You must specify the tweet to remove");
    }
}

internal sealed class RemoveTweetHandler : ICommandHandler<RemoveTweetCommand, Result<Unit>>
{
    private readonly AppDbContext _repository;
    public RemoveTweetHandler(AppDbContext repository) => _repository = repository;

    public async Task<Result<Unit>> Handle(RemoveTweetCommand request, CancellationToken ct)
    {
        var tweet = await _repository.Set<Tweet>().SingleAsync(c => c.Id == TweetId.From(request.TweetId), ct);
        _repository.Remove(tweet);
        
        var results= await _repository.SaveChangesAsync(ct);
        return results > 0 ? new Result<Unit>(Unit.Value):new Result<Unit>(new Error("Failed to remove tweet"));
    }
}