using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OSS.Twtr.App.Domain.Entities;
using OSS.Twtr.App.Domain.Repositories;
using OSS.Twtr.App.Domain.ValueObjects;
using OSS.Twtr.App.Infrastructure;
using OSS.Twtr.Application;
using OSS.Twtr.Core;

namespace OSS.Twtr.App.Application;

public record struct UnpinTweetCommand(Guid UserId, Guid TweetId) : ICommand<Result<Unit>>;

public sealed class UnpinTweetValidator : AbstractValidator<UnpinTweetCommand>
{
    public UnpinTweetValidator()
    {
        RuleFor(x => x.UserId).NotEqual(Guid.Empty)
            .WithMessage("You must be authenticated to unpin a Tweet");

        RuleFor(x => x.TweetId).NotEqual(Guid.Empty)
            .WithMessage("You must specify the tweet to unpin");
    }
}

internal sealed class UnpinTweetHandler : ICommandHandler<UnpinTweetCommand, Result<Unit>>
{
    private readonly AppDbContext _repository;
    public UnpinTweetHandler(AppDbContext repository) => _repository = repository;

    public async Task<Result<Unit>> Handle(UnpinTweetCommand request, CancellationToken ct)
    {
        var author = await _repository.Set<User>().SingleAsync(b => 
            b.Id == UserId.From(request.UserId), ct);
        
        author.UnpinTweet(TweetId.From(request.TweetId));
        
        var results= await _repository.SaveChangesAsync(ct);
        return results > 0 ? new Result<Unit>(Unit.Value):new Result<Unit>(new Error("Failed to unpin tweet"));
    }
}