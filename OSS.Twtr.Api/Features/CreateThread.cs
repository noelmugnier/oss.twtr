using System.Security.Claims;
using OSS.Twtr.App.Application;
using OSS.Twtr.Application;
using OSS.Twtr.Common.Infrastructure;

namespace OSS.Twtr.Api.Features;

public record CreateThreadRequest
{
    public IEnumerable<string> Messages { get; init; }
}

public sealed class CreateThreadEndpoint : TwtrEndpoint<CreateThreadRequest, Guid>
{
    private readonly ICommandDispatcher _mediator;
    public CreateThreadEndpoint(ICommandDispatcher mediator) => _mediator = mediator;

    public override void Configure()
    {
        Post("/threads");
        Policies("auth");
    }

    public override async Task HandleAsync(CreateThreadRequest req, CancellationToken ct)
    {
        var result = await _mediator.Execute(
            new CreateThreadCommand(Guid.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value),
                req.Messages), ct);

        await result.On(
            threadId => SendOkAsync(cancellation: ct),
            errors => SendResultErrorsAsync(errors, ct));
    }
}