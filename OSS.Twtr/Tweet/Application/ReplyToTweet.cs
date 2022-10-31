using FluentValidation;
using OSS.Twtr.Common.Application;
using OSS.Twtr.Common.Core;
using OSS.Twtr.Tweet.Domain.Repositories;
using OSS.Twtr.Tweet.Domain.ValueObjects;

namespace OSS.Twtr.Tweet.Application;

public record struct ReplyToTweetCommand(Guid UserId, Guid TweetId, string Message) : ICommand<Result<Guid>>;

public sealed class ReplyToTweetValidator : AbstractValidator<ReplyToTweetCommand>
{
    public ReplyToTweetValidator()
    {
        RuleFor(x => x.Message).NotEmpty()
            .WithMessage("Tweet message cannot be empty");

        RuleFor(x => x.UserId).NotEqual(Guid.Empty)
            .WithMessage("You must be authenticated to reply to a tweet");

        RuleFor(x => x.TweetId).NotEqual(Guid.Empty)
            .WithMessage("You must specify a tweet to reply to");
    }
}

internal sealed class ReplyToTweetHandler : ICommandHandler<ReplyToTweetCommand, Result<Guid>>
{
    private readonly ITweetRepository _repository;
    public ReplyToTweetHandler(ITweetRepository repository) => _repository = repository;

    public async Task<Result<Guid>> Handle(ReplyToTweetCommand request, CancellationToken ct)
    {
        var userResult = await _repository.Get(UserId.From(request.UserId), ct);
        if (!userResult.Success)
            return new Result<Guid>(userResult.Errors);

        var author = userResult.Data;
        var replyId = author.ReplyTo(TweetId.From(request.TweetId), request.Message);

        await _repository.Save(author, ct);

        return new Result<Guid>(replyId.Value);
    }
}