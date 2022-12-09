using FluentValidation;
using OSS.Twtr.App.Domain.Entities;
using OSS.Twtr.App.Domain.Repositories;
using OSS.Twtr.App.Domain.ValueObjects;
using OSS.Twtr.App.Infrastructure;
using OSS.Twtr.Application;
using OSS.Twtr.Core;

namespace OSS.Twtr.App.Application;

public record struct BookmarkTweetCommand(Guid UserId, Guid TweetId) : ICommand<Result<Unit>>;

public sealed class BookmarkTweetValidator : AbstractValidator<BookmarkTweetCommand>
{
    public BookmarkTweetValidator()
    {
        RuleFor(x => x.UserId).NotEqual(Guid.Empty)
            .WithMessage("You must be authenticated to bookmark a Tweet");

        RuleFor(x => x.TweetId).NotEqual(Guid.Empty)
            .WithMessage("You must specify the tweet to bookmark");
    }
}

internal sealed class BookmarkTweetHandler : ICommandHandler<BookmarkTweetCommand, Result<Unit>>
{
    private readonly AppDbContext _repository;
    public BookmarkTweetHandler(AppDbContext repository) => _repository = repository;

    public async Task<Result<Unit>> Handle(BookmarkTweetCommand request, CancellationToken ct)
    {
        var bookmark = Bookmark.Create(UserId.From(request.UserId), TweetId.From(request.TweetId));
        await _repository.AddAsync(bookmark, ct);    
        
        var results= await _repository.SaveChangesAsync(ct);
        return results > 0 ? new Result<Unit>(Unit.Value):new Result<Unit>(new Error("Failed to bookmark tweet"));
    }
}