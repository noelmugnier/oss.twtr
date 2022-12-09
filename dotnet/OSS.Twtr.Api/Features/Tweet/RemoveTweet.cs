using System.Security.Claims;
using OSS.Twtr.App.Application;
using OSS.Twtr.Application;
using OSS.Twtr.Common.Infrastructure;

namespace OSS.Twtr.Api.Features;

public record RemoveTweetRequest
{
    public Guid TweetId { get; init; }
}

public sealed class RemoveTweetEndpoint : TwtrEndpoint<RemoveTweetRequest>
{
    private readonly ICommandDispatcher _mediator;
    public RemoveTweetEndpoint(ICommandDispatcher mediator) => _mediator = mediator;

    public override void Configure()
    {
        Delete("/tweets/{TweetId:Guid}");
        Policies("auth");
    }

    public override async Task HandleAsync(RemoveTweetRequest req, CancellationToken ct)
    {
        var result = await _mediator.Execute(
            new RemoveTweetCommand(Guid.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value),
                req.TweetId), ct);

        await result.On(
            success => SendOkAsync(cancellation: ct),
            errors => SendResultErrorsAsync(errors, ct));
    }
}