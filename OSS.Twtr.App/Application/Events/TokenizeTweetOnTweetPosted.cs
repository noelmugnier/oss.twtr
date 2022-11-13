using OSS.Twtr.App.Domain.Events;
using OSS.Twtr.Application;

namespace OSS.Twtr.App.Application;

internal sealed class TokenizeTweetOnTweetPostedHandler : IEventHandler<WrappedDomainEvent<TweetPosted>>
{
    private readonly ICommandDispatcher _commandDispatcher;
    public TokenizeTweetOnTweetPostedHandler(ICommandDispatcher commandDispatcher) => _commandDispatcher = commandDispatcher;

    public async Task Handle(WrappedDomainEvent<TweetPosted> notification, CancellationToken token)
    {
        var results = await _commandDispatcher.Execute(new TokenizeTweetCommand(notification.DomainEvent.TweetId), token);
        if (!results.Success)
            throw new Exception(results.Errors.Aggregate("", (acc, error) => acc + error.Message + "\n"));
    }
}