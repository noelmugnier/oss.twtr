using FastEndpoints;
using FluentValidation;
using Mapster;
using MediatR;
using OSS.Twtr.Management.Application.Queries;
using OSS.Twtr.Management.Domain.Contracts;

namespace OSS.Twtr.Management.Infrastructure.Endpoints;

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

public record struct GetTweetResponse(Guid Id, string Message, DateTimeOffset PostedOn, GetTweetUserResponse User);
public record struct GetTweetUserResponse(Guid Id, string UserName, string DisplayName);