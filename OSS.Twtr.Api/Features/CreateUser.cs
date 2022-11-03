using Mapster;
using OSS.Twtr.Application;
using OSS.Twtr.Auth.Application;
using OSS.Twtr.Common.Infrastructure;

namespace OSS.Twtr.Api.Features;

public record CreateUserRequest
{
    public string Username { get; init; }
    public string Password { get; init; }
    public string ConfirmPassword { get; init; }
}

public sealed class CreateUserEndpoint : TwtrEndpoint<CreateUserRequest, Guid>
{
    private readonly ICommandDispatcher _mediator;
    public CreateUserEndpoint(ICommandDispatcher mediator) => _mediator = mediator;

    public override void Configure()
    {
        Post("/auth/register");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateUserRequest req, CancellationToken ct)
    {
        var result = await _mediator.Execute(req.Adapt<CreateUserCommand>(), ct);
        await result.On(
            createdUserId => SendAsync(createdUserId, cancellation: ct),
            errors => SendResultErrorsAsync(errors, ct));
    }
}