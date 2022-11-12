using System.Security.Claims;
using OSS.Twtr.App.Application;
using OSS.Twtr.Application;
using OSS.Twtr.Common.Infrastructure;

namespace OSS.Twtr.Api.Features;

public record GetUserRequest
{
    public Guid UserId { get; init; }
}

public sealed class GetUserEndpoint : TwtrEndpoint<GetUserRequest>
{
    private readonly IQueryDispatcher _mediator;
    public GetUserEndpoint(IQueryDispatcher mediator) => _mediator = mediator;

    public override void Configure()
    {
        Get("/users/{UserId:Guid}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetUserRequest req, CancellationToken ct)
    {
        var result = await _mediator.Execute(
            new GetUserQuery(req.UserId, User.Identity.IsAuthenticated ? Guid.Parse(User.Claims.First(c => c.Type == ClaimTypes
            .NameIdentifier).Value) : null), ct);

        await result.On(
            user => SendAsync(user, cancellation: ct),
            errors => SendResultErrorsAsync(errors, ct));
    }
}