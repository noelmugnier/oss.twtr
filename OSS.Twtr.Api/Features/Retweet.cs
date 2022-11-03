using System.Security.Claims;
using OSS.Twtr.App.Application;
using OSS.Twtr.Application;
using OSS.Twtr.Common.Infrastructure;

namespace OSS.Twtr.Api.Features;

public record RetweetRequest
{
    public Guid TweetId { get; init; }
}

public sealed class RetweetEndpoint : TwtrEndpoint<RetweetRequest>
{
    private readonly ICommandDispatcher _mediator;
    public RetweetEndpoint(ICommandDispatcher mediator) => _mediator = mediator;

    public override void Configure()
    {
        Post("/tweets/{TweetId:Guid}/retweet");
        Policies("auth");
    }

    public override async Task HandleAsync(RetweetRequest req, CancellationToken ct)
    {
        var result = await _mediator.Execute(
            new RetweetCommand(Guid.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value),
                req.TweetId), ct);

        await result.On(
            tweetId => SendOkAsync(cancellation: ct),
            errors => SendResultErrorsAsync(errors, ct));
    }
}