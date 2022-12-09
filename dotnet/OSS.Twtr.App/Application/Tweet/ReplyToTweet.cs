using FluentValidation;
using LinqToDB.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OSS.Twtr.App.Domain.Entities;
using OSS.Twtr.App.Domain.Enums;
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
        var block = await _repository.Set<Block>().SingleOrDefaultAsync(c => c.UserId == tweet.AuthorId && c.UserIdToBlock == UserId.From(request.UserId), ct);
        if (block != null)
            return new Result<Unit>(new Error("Cannot reply to this tweet, author blocked you"));
        
        var canReplyResult = tweet.AllowedReplies switch
        {
            TweetAllowedReplies.Following => await CheckIfReplierIsFollowedByAuthor(UserId.From(request.UserId), tweet.AuthorId, ct),
            TweetAllowedReplies.Mentioned => await CheckIfReplierIsMentionedInTweet(UserId.From(request.UserId), tweet, ct),
            _ => new Result<Unit>(Unit.Value)
        };

        if (!canReplyResult.Success)
            return canReplyResult;
        
        var reply = tweet.Reply(request.Message, UserId.From(request.UserId));
        await _repository.AddAsync(reply, ct);    

        var results= await _repository.SaveChangesAsync(ct);
        return results > 0 ? new Result<Unit>(Unit.Value):new Result<Unit>(new Error("Failed to reply to tweet"));
    }

    private async Task<Result<Unit>> CheckIfReplierIsMentionedInTweet(UserId replierId, Tweet tweet, CancellationToken token)
    {
        var username = await _repository.Set<User>()
            .Where(a => a.Id == replierId)
            .Select(a => a.UserName)
            .SingleAsync(token);
        
        var replierIsMentioned = tweet.Message?.Contains($"@{username}");
        return replierIsMentioned switch
        {
            true => new Result<Unit>(Unit.Value),
            _ => new Result<Unit>(new Error("You must be mentioned in the tweet to reply to it"))
        };
    }

    private async Task<Result<Unit>> CheckIfReplierIsFollowedByAuthor(UserId replierId, UserId authorId, CancellationToken token)
    {
        var replierIsFollowedByAuthor = await _repository.Set<Subscription>().AnyAsync(s => s.SubscribedToUserId ==
            replierId && s.FollowerUserId == authorId, token);

        return replierIsFollowedByAuthor switch
        {
            true => new Result<Unit>(Unit.Value),
            _ => new Result<Unit>(new Error("You must be followed by the author to reply to his tweet"))
        };
    }
}