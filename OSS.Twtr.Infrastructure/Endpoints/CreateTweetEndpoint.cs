using System.Security.Claims;
using FastEndpoints;
using FluentValidation;
using Mapster;
using MediatR;
using OSS.Twtr.Management.Application.Commands;

namespace OSS.Twtr.Management.Infrastructure.Endpoints;

public class CreateTweetEndpoint : Endpoint<CreateTweetRequest, CreateTweetResponse>
{
    private readonly IMediator _mediator;

    public CreateTweetEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/tweet");
        Policies("auth");
    }

    public override async Task HandleAsync(CreateTweetRequest req, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateTweetCommand(Guid.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value), req.Message), ct);
        await result.On(success => SendAsync(success.Adapt<CreateTweetResponse>(), cancellation: ct), errors =>
        {
            foreach (var error in errors)
                AddError(error.Message, error.ErrorCode, (Severity)error.Severity);

            return SendErrorsAsync(cancellation: ct);
        });
    }
}

public record struct CreateTweetRequest(string Message);
public record struct CreateTweetResponse(Guid Id, string Message, string UserName, string DisplayName, DateTimeOffset PostedOn);