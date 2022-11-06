using System.Security.Claims;
using OSS.Twtr.App.Application;
using OSS.Twtr.Application;
using OSS.Twtr.Common.Infrastructure;

namespace OSS.Twtr.Api.Features;

public record BlockUserRequest
{
    public Guid UserId { get; init; }
}

public sealed class BlockUserEndpoint : TwtrEndpoint<BlockUserRequest>
{
    private readonly ICommandDispatcher _mediator;
    public BlockUserEndpoint(ICommandDispatcher mediator) => _mediator = mediator;

    public override void Configure()
    {
        Put("/users/{UserId:Guid}/block");
        Policies("auth");
    }

    public override async Task HandleAsync(BlockUserRequest req, CancellationToken ct)
    {
        var result = await _mediator.Execute(
            new BlockUserCommand(req.UserId, Guid.Parse(User.Claims.First(c => c.Type == ClaimTypes
            .NameIdentifier).Value)), ct);

        await result.On(
            success => SendOkAsync(cancellation: ct),
            errors => SendResultErrorsAsync(errors, ct));
    }
}