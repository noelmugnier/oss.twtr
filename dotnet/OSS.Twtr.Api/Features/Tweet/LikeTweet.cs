using System.Security.Claims;
using OSS.Twtr.App.Application;
using OSS.Twtr.Application;
using OSS.Twtr.Common.Infrastructure;

namespace OSS.Twtr.Api.Features;

public record LikeTweetRequest
{
    public Guid TweetId { get; init; }
}

public sealed class LikeTweetEndpoint : TwtrEndpoint<LikeTweetRequest>
{
    private readonly ICommandDispatcher _mediator;
    public LikeTweetEndpoint(ICommandDispatcher mediator) => _mediator = mediator;

    public override void Configure()
    {
        Put("/tweets/{TweetId:Guid}/like");
        Policies("auth");
    }

    public override async Task HandleAsync(LikeTweetRequest req, CancellationToken ct)
    {
        var result = await _mediator.Execute(
            new LikeTweetCommand(Guid.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value),
                req.TweetId), ct);

        await result.On(
            tweetId => SendOkAsync(cancellation: ct),
            errors => SendResultErrorsAsync(errors, ct));
    }
}