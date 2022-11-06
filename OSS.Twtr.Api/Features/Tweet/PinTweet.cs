using System.Security.Claims;
using OSS.Twtr.App.Application;
using OSS.Twtr.Application;
using OSS.Twtr.Common.Infrastructure;

namespace OSS.Twtr.Api.Features;

public record PinTweetRequest
{
    public Guid TweetId { get; init; }
}

public sealed class PinTweetEndpoint : TwtrEndpoint<PinTweetRequest>
{
    private readonly ICommandDispatcher _mediator;
    public PinTweetEndpoint(ICommandDispatcher mediator) => _mediator = mediator;

    public override void Configure()
    {
        Put("/tweets/{TweetId:Guid}/pin");
        Policies("auth");
    }

    public override async Task HandleAsync(PinTweetRequest req, CancellationToken ct)
    {
        var result = await _mediator.Execute(
            new PinTweetCommand(Guid.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value),
                req.TweetId), ct);

        await result.On(
            tweetId => SendOkAsync(cancellation: ct),
            errors => SendResultErrorsAsync(errors, ct));
    }
}