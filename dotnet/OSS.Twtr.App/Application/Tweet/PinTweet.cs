using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OSS.Twtr.App.Domain.Entities;
using OSS.Twtr.App.Domain.Repositories;
using OSS.Twtr.App.Domain.ValueObjects;
using OSS.Twtr.App.Infrastructure;
using OSS.Twtr.Application;
using OSS.Twtr.Core;

namespace OSS.Twtr.App.Application;

public record struct PinTweetCommand(Guid TweetId, Guid UserId) : ICommand<Result<Unit>>;

public sealed class PinTweetValidator : AbstractValidator<PinTweetCommand>
{
    public PinTweetValidator()
    {
        RuleFor(x => x.UserId).NotEqual(Guid.Empty)
            .WithMessage("You must be authenticated to pin a tweet");

        RuleFor(x => x.TweetId).NotEmpty()
            .WithMessage("You must specify the tweet to pin");
    }
}

internal sealed class PinTweetHandler : ICommandHandler<PinTweetCommand, Result<Unit>>
{
    private readonly AppDbContext _repository;
    public PinTweetHandler(AppDbContext repository) => _repository = repository;

    public async Task<Result<Unit>> Handle(PinTweetCommand request, CancellationToken ct)
    {
        var author = await _repository.Set<User>()
            .SingleAsync(c => c.Id == UserId.From(request.UserId), ct);
        
        author.PinTweet(TweetId.From(request.TweetId));
        
        var results= await _repository.SaveChangesAsync(ct);
        return results > 0 ? new Result<Unit>(Unit.Value):new Result<Unit>(new Error("Failed to pin tweet"));
    }
}