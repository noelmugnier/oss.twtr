using System.Security.Claims;
using OSS.Twtr.App.Application;
using OSS.Twtr.Application;
using OSS.Twtr.Common.Infrastructure;

namespace OSS.Twtr.Api.Features;

public record UnpinTweetRequest
{
    public Guid TweetId { get; init; }
}

public sealed class UnpinTweetEndpoint : TwtrEndpoint<UnpinTweetRequest>
{
    private readonly ICommandDispatcher _mediator;
    public UnpinTweetEndpoint(ICommandDispatcher mediator) => _mediator = mediator;

    public override void Configure()
    {
        Put("/tweets/{TweetId:Guid}/unpin");
        Policies("auth");
    }

    public override async Task HandleAsync(UnpinTweetRequest req, CancellationToken ct)
    {
        var result = await _mediator.Execute(
            new UnpinTweetCommand(Guid.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value),
                req.TweetId), ct);

        await result.On(
            tweetId => SendOkAsync(cancellation: ct),
            errors => SendResultErrorsAsync(errors, ct));
    }
}