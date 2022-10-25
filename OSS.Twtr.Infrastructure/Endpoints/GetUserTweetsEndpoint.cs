using FastEndpoints;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OSS.Twtr.Management.Application.Queries;
using OSS.Twtr.Domain.Contracts;

namespace OSS.Twtr.Management.Infrastructure.Endpoints;

public class GetUserTweetsEndpoint : Endpoint<GetUserTweetsRequest, GetUserTweetsResponse>
{
    private readonly IMediator _mediator;

    public GetUserTweetsEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/users/{UserId:Guid}/tweets");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetUserTweetsRequest req, CancellationToken ct)
    {
        var result = await _mediator.Send(req.Adapt<GetUserTweetsQuery>(), ct);
        await result.On(success => SendAsync(new GetUserTweetsResponse(success), cancellation: ct), errors =>
        {
            foreach (var error in errors)
                AddError(error.Message, error.ErrorCode, (Severity)error.Severity);

            return SendErrorsAsync(cancellation: ct);
        });
    }
}

public record GetUserTweetsRequest
{
    [FromRoute]
    public Guid UserId { get; init; }
    [FromQuery]
    public int Page { get; init; }
    [FromQuery]
    public int Count { get; init; }
}

public record struct GetUserTweetsResponse(IEnumerable<UserTweetDto> Tweets);