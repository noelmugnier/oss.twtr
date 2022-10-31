using System.Security.Claims;
using OSS.Twtr.Common.Application;
using OSS.Twtr.Common.Infrastructure;
using OSS.Twtr.Tweet.Application;

namespace OSS.Twtr.Tweet.Infrastructure;

public record ReplyToTweetRequest
{
    public Guid TweetId { get; set; }
    public string Message { get; init; }
}

public sealed class ReplyToTweetEndpoint : TwtrEndpoint<ReplyToTweetRequest, Guid>
{
    private readonly ICommandDispatcher _mediator;
    public ReplyToTweetEndpoint(ICommandDispatcher mediator) => _mediator = mediator;

    public override void Configure()
    {
        Post("/tweets/{TweetId:Guid}");
        Policies("auth");
    }

    public override async Task HandleAsync(ReplyToTweetRequest req, CancellationToken ct)
    {
        var result = await _mediator.Execute(
            new ReplyToTweetCommand(Guid.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value),
                req.TweetId, req.Message), ct);

        await result.On(
            tweetId => SendCreatedAtAsync<GetTweetEndpoint>(new {TweetId = tweetId}, tweetId, cancellation: ct),
            errors => SendResultErrorsAsync(errors, ct));
    }
}