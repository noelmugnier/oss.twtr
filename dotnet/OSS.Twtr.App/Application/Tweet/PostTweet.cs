using FluentValidation;
using OSS.Twtr.App.Domain.Entities;
using OSS.Twtr.App.Domain.Enums;
using OSS.Twtr.App.Domain.Repositories;
using OSS.Twtr.App.Domain.ValueObjects;
using OSS.Twtr.App.Infrastructure;
using OSS.Twtr.Application;
using OSS.Twtr.Core;

namespace OSS.Twtr.App.Application;

public record struct PostTweetCommand(Guid UserId, string Message, TweetAllowedReplies AllowedReplies) : 
ICommand<Result<Unit>>;

internal sealed class PostTweetValidator : AbstractValidator<PostTweetCommand>
{
    public PostTweetValidator()
    {
        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Tweet content is required")
            .MaximumLength(140).WithMessage("Tweet is limited to 140 characters");
        RuleFor(x => x.UserId).NotEqual(Guid.Empty)
            .WithMessage("You must be authenticated to perform this action");
    }
}

internal sealed class PostTweetHandler : ICommandHandler<PostTweetCommand, Result<Unit>>
{
    private readonly AppDbContext _repository;
    public PostTweetHandler(AppDbContext repository) => _repository = repository;

    public async Task<Result<Unit>> Handle(PostTweetCommand request, CancellationToken ct)
    {
        var tweet = Tweet.Create(request.Message, UserId.From(request.UserId), request.AllowedReplies);
        await _repository.AddAsync(tweet, ct);

        var results = await _repository.SaveChangesAsync(ct);
        return results > 0 ? new Result<Unit>(Unit.Value) : new Result<Unit>(new Error("Failed to post tweet"));
    }
}