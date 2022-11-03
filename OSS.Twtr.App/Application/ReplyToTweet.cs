using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OSS.Twtr.App.Domain.Entities;
using OSS.Twtr.App.Domain.Repositories;
using OSS.Twtr.App.Domain.ValueObjects;
using OSS.Twtr.App.Infrastructure;
using OSS.Twtr.Application;
using OSS.Twtr.Core;

namespace OSS.Twtr.App.Application;

public record struct ReplyToTweetCommand(Guid UserId, Guid TweetId, string Message) : ICommand<Result<Unit>>;

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

internal sealed class ReplyToTweetHandler : ICommandHandler<ReplyToTweetCommand, Result<Unit>>
{
    private readonly AppDbContext _repository;
    public ReplyToTweetHandler(AppDbContext repository) => _repository = repository;

    public async Task<Result<Unit>> Handle(ReplyToTweetCommand request, CancellationToken ct)
    {
        var tweet = await _repository.Set<Tweet>().SingleAsync(c => c.Id == TweetId.From(request.TweetId), ct);
        var quote = tweet.Reply(request.Message, UserId.From(request.UserId));
        
        await _repository.AddAsync(quote, ct);    

        var results= await _repository.SaveChangesAsync(ct);
        return results > 0 ? new Result<Unit>(Unit.Value):new Result<Unit>(new Error("Failed to reply to tweet"));
    }
}