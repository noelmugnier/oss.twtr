using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OSS.Twtr.App.Domain.Entities;
using OSS.Twtr.App.Domain.Repositories;
using OSS.Twtr.App.Domain.ValueObjects;
using OSS.Twtr.App.Infrastructure;
using OSS.Twtr.Application;
using OSS.Twtr.Core;

namespace OSS.Twtr.App.Application;

public record struct UnbookmarkTweetCommand(Guid UserId, Guid TweetId) : ICommand<Result<Unit>>;

public sealed class UnbookmarkTweetValidator : AbstractValidator<UnbookmarkTweetCommand>
{
    public UnbookmarkTweetValidator()
    {
        RuleFor(x => x.UserId).NotEqual(Guid.Empty)
            .WithMessage("You must be authenticated to unbookmark a Tweet");

        RuleFor(x => x.TweetId).NotEqual(Guid.Empty)
            .WithMessage("You must specify the tweet to unbookmark");
    }
}

internal sealed class UnbookmarkTweetHandler : ICommandHandler<UnbookmarkTweetCommand, Result<Unit>>
{
    private readonly AppDbContext _repository;
    public UnbookmarkTweetHandler(AppDbContext repository) => _repository = repository;

    public async Task<Result<Unit>> Handle(UnbookmarkTweetCommand request, CancellationToken ct)
    {
        var bookmark = await _repository.Set<Bookmark>().SingleOrDefaultAsync(b => 
            b.UserId == UserId.From(request.UserId) && b.TweetId == TweetId.From(request.TweetId), ct);

        if (bookmark == null)
            return new Result<Unit>(Unit.Value);
        
        _repository.Remove(bookmark);
        
        var results= await _repository.SaveChangesAsync(ct);
        return results > 0 ? new Result<Unit>(Unit.Value):new Result<Unit>(new Error("Failed to unbookmark tweet"));
    }
}