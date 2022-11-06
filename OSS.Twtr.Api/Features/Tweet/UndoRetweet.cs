using System.Security.Claims;
using OSS.Twtr.App.Application;
using OSS.Twtr.Application;
using OSS.Twtr.Common.Infrastructure;

namespace OSS.Twtr.Api.Features;

public record UndoRetweetRequest
{
    public Guid TweetId { get; init; }
}

public sealed class UndoRetweetEndpoint : TwtrEndpoint<UndoRetweetRequest>
{
    private readonly ICommandDispatcher _mediator;
    public UndoRetweetEndpoint(ICommandDispatcher mediator) => _mediator = mediator;

    public override void Configure()
    {
        Put("/tweets/{TweetId:Guid}/retweet/undo");
        Policies("auth");
    }

    public override async Task HandleAsync(UndoRetweetRequest req, CancellationToken ct)
    {
        var result = await _mediator.Execute(
            new UndoRetweetCommand(Guid.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value),
                req.TweetId), ct);

        await result.On(
            tweetId => SendOkAsync(cancellation: ct),
            errors => SendResultErrorsAsync(errors, ct));
    }
}