using System.Security.Claims;
using OSS.Twtr.App.Application;
using OSS.Twtr.Application;
using OSS.Twtr.Common.Infrastructure;

namespace OSS.Twtr.Api.Features;

public record UnblockUserRequest
{
    public Guid UserId { get; init; }
}

public sealed class UnblockUserEndpoint : TwtrEndpoint<UnblockUserRequest>
{
    private readonly ICommandDispatcher _mediator;
    public UnblockUserEndpoint(ICommandDispatcher mediator) => _mediator = mediator;

    public override void Configure()
    {
        Put("/users/{UserId:Guid}/unblock");
        Policies("auth");
    }

    public override async Task HandleAsync(UnblockUserRequest req, CancellationToken ct)
    {
        var result = await _mediator.Execute(
            new UnblockUserCommand(req.UserId, Guid.Parse(User.Claims.First(c => c.Type == ClaimTypes
            .NameIdentifier).Value)), ct);

        await result.On(
            success => SendOkAsync(cancellation: ct),
            errors => SendResultErrorsAsync(errors, ct));
    }
}