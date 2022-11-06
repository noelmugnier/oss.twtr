using System.Security.Claims;
using OSS.Twtr.App.Application;
using OSS.Twtr.Application;
using OSS.Twtr.Common.Infrastructure;

namespace OSS.Twtr.Api.Features;

public record UnlikeTweetRequest
{
    public Guid TweetId { get; init; }
}

public sealed class UnlikeTweetEndpoint : TwtrEndpoint<UnlikeTweetRequest>
{
    private readonly ICommandDispatcher _mediator;
    public UnlikeTweetEndpoint(ICommandDispatcher mediator) => _mediator = mediator;

    public override void Configure()
    {
        Put("/tweets/{TweetId:Guid}/unlike");
        Policies("auth");
    }

    public override async Task HandleAsync(UnlikeTweetRequest req, CancellationToken ct)
    {
        var result = await _mediator.Execute(
            new UnlikeTweetCommand(Guid.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value),
                req.TweetId), ct);

        await result.On(
            tweetId => SendOkAsync(cancellation: ct),
            errors => SendResultErrorsAsync(errors, ct));
    }
}