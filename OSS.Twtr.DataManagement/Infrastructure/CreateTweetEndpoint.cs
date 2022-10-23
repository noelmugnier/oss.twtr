using FastEndpoints;
using FluentValidation;
using Mapster;
using MediatR;
using OSS.Twtr.TweetManagement.Application;

namespace OSS.Twtr.TweetManagement.Infrastructure;

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
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateTweetRequest req, CancellationToken ct)
    {
        var result = await _mediator.Send(req.Adapt<CreateTweetCommand>(), ct);
        await result.On(success => SendAsync(success.Adapt<CreateTweetResponse>(), cancellation: ct), errors =>
        {
            foreach (var error in errors)
                AddError(error.Message, error.ErrorCode, (Severity)error.Severity);

            return SendErrorsAsync(cancellation: ct);
        });
    }
}

public record struct CreateTweetRequest(Guid UserId, string Message);
public record struct CreateTweetResponse(Guid Id, string Message, string UserName, string DisplayName, DateTimeOffset PostedOn);