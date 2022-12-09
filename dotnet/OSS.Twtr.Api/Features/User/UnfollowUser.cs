using System.Security.Claims;
using OSS.Twtr.App.Application;
using OSS.Twtr.Application;
using OSS.Twtr.Common.Infrastructure;

namespace OSS.Twtr.Api.Features;

public record UnfollowUserRequest
{
    public Guid UserId { get; init; }
}

public sealed class UnfollowUserEndpoint : TwtrEndpoint<UnfollowUserRequest>
{
    private readonly ICommandDispatcher _mediator;
    public UnfollowUserEndpoint(ICommandDispatcher mediator) => _mediator = mediator;

    public override void Configure()
    {
        Put("/users/{UserId:Guid}/unfollow");
        Policies("auth");
    }

    public override async Task HandleAsync(UnfollowUserRequest req, CancellationToken ct)
    {
        var result = await _mediator.Execute(
            new UnfollowUserCommand(req.UserId, Guid.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier)
            .Value)), ct);

        await result.On(
            success => SendOkAsync(cancellation: ct),
            errors => SendResultErrorsAsync(errors, ct));
    }
}