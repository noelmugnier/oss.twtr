using System.Security.Claims;
using OSS.Twtr.App.Application;
using OSS.Twtr.Application;
using OSS.Twtr.Common.Infrastructure;

namespace OSS.Twtr.Api.Features;

public record ReplyToTweetRequest
{
    public Guid TweetId { get; init; }
    public string Message { get; init; }
}

public sealed class ReplyToTweetEndpoint : TwtrEndpoint<ReplyToTweetRequest>
{
    private readonly ICommandDispatcher _mediator;
    public ReplyToTweetEndpoint(ICommandDispatcher mediator) => _mediator = mediator;

    public override void Configure()
    {
        Post("/tweets/{TweetId:Guid}/reply");
        Policies("auth");
    }

    public override async Task HandleAsync(ReplyToTweetRequest req, CancellationToken ct)
    {
        var result = await _mediator.Execute(
            new ReplyToTweetCommand(Guid.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value),
                req.TweetId, req.Message), ct);

        await result.On(
            tweetId => SendOkAsync(cancellation: ct),
            errors => SendResultErrorsAsync(errors, ct));
    }
}