using Microsoft.EntityFrameworkCore;
using OSS.Twtr.App.Domain.Entities;
using OSS.Twtr.App.Domain.Events;
using OSS.Twtr.App.Domain.ValueObjects;
using OSS.Twtr.App.Infrastructure;
using OSS.Twtr.Application;

namespace OSS.Twtr.App.Application;

internal sealed class AiReplyOnTweetPostedHandler : IEventHandler<WrappedDomainEvent<TweetPosted>>
{
    private readonly AppDbContext _repository;
    private readonly ICommandDispatcher _commandDispatcher;
    public AiReplyOnTweetPostedHandler(AppDbContext repository, ICommandDispatcher commandDispatcher)
    {
        _repository = repository;
        _commandDispatcher = commandDispatcher;
    }

    public async Task Handle(WrappedDomainEvent<TweetPosted> notification, CancellationToken token)
    {
        var tweet = await _repository.Set<Tweet>().SingleOrDefaultAsync(t => t.Id == TweetId.From(notification
            .DomainEvent.TweetId), token);
        if (tweet == null)
            return;
        
        if (tweet.AuthorId == AiUsers.Negative || tweet.AuthorId == AiUsers.Neutral || tweet.AuthorId == AiUsers.Positive)
            return;
        
        var aiReplyResult = await _commandDispatcher.Execute(new GenerateAiReplyCommand(notification.DomainEvent.TweetId), token);
        if (!aiReplyResult.Success)
            throw new Exception(aiReplyResult.Errors.Aggregate("", (acc, error) => acc + error.Message + "\n"));

        await _commandDispatcher.Execute(new ReplyToTweetCommand(aiReplyResult.Data.ReplierId, notification.DomainEvent.TweetId, aiReplyResult.Data.Message), token);
    }
}

internal sealed class AiReplyOnTweetReplyHandler : IEventHandler<WrappedDomainEvent<TweetReplied>>
{
    private readonly AppDbContext _repository;
    private readonly ICommandDispatcher _commandDispatcher;
    public AiReplyOnTweetReplyHandler(AppDbContext repository, ICommandDispatcher commandDispatcher)
    {
        _repository = repository;
        _commandDispatcher = commandDispatcher;
    }

    public async Task Handle(WrappedDomainEvent<TweetReplied> notification, CancellationToken token)
    {
        var tweet = await _repository.Set<Tweet>().SingleOrDefaultAsync(t => t.Id == TweetId.From(notification
            .DomainEvent.ReplyTweetId), token);
        if (tweet == null)
            return;
        
        if (tweet.AuthorId == AiUsers.Negative || tweet.AuthorId == AiUsers.Neutral || tweet.AuthorId == AiUsers.Positive)
            return;

        var aiReplyResult = await _commandDispatcher.Execute(new GenerateAiReplyCommand(notification.DomainEvent.ReplyTweetId), token);
        if (!aiReplyResult.Success)
            throw new Exception(aiReplyResult.Errors.Aggregate("", (acc, error) => acc + error.Message + "\n"));

        await _commandDispatcher.Execute(new ReplyToTweetCommand(aiReplyResult.Data.ReplierId, notification.DomainEvent.RepliedTweetId, aiReplyResult.Data.Message), token);
    }
}