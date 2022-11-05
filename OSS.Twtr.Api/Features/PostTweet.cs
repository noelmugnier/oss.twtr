using System.Security.Claims;
using OSS.Twtr.App.Application;
using OSS.Twtr.Application;
using OSS.Twtr.Common.Infrastructure;

namespace OSS.Twtr.Api.Features;

public record PostTweetRequest
{
    public string Message { get; init; }
}

public sealed class PostTweetEndpoint : TwtrEndpoint<PostTweetRequest>
{
    private readonly ICommandDispatcher _mediator;
    public PostTweetEndpoint(ICommandDispatcher mediator) => _mediator = mediator;

    public override void Configure()
    {
        Post("/tweets");
        Policies("auth");
    }

    public override async Task HandleAsync(PostTweetRequest req, CancellationToken ct)
    {
        var result = await _mediator.Execute(
            new PostTweetCommand(Guid.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value),
                req.Message), ct);

        await result.On(
            success => SendOkAsync(cancellation: ct),
            errors => SendResultErrorsAsync(errors, ct));
    }
}