using FastEndpoints;
using FluentValidation;
using Mapster;
using MediatR;
using OSS.Twtr.TweetManagement.Application;
using OSS.Twtr.TweetManagement.Domain;

namespace OSS.Twtr.TweetManagement.Infrastructure;

public class GetTweetEndpoint : Endpoint<GetTweetRequest, GetTweetResponse>
{
    private readonly IMediator _mediator;

    public GetTweetEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/tweet/{TweetId:Guid}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetTweetRequest req, CancellationToken ct)
    {
        var result = await _mediator.Send(req.Adapt<GetTweetQuery>(), ct);
        await result.On(success => SendAsync(success.Adapt<GetTweetResponse>(), cancellation: ct), errors =>
        {
            foreach (var error in errors)
                AddError(error.Message, error.ErrorCode, (Severity)error.Severity);

            return SendErrorsAsync(cancellation: ct);
        });
    }
}

public record GetTweetRequest
{
    public Guid TweetId { get; init; }
}

public record struct GetTweetResponse(Guid Id, string UserName, string DisplayName, string Message, DateTimeOffset PostedOn);