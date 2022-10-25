using FastEndpoints;
using FluentValidation;
using Mapster;
using MediatR;
using OSS.Twtr.Management.Application.Queries;
using OSS.Twtr.Domain.Contracts;

namespace OSS.Twtr.Management.Infrastructure.Endpoints;

public class GetUserProfileEndpoint : Endpoint<GetUserProfileRequest, GetUserProfileResponse>
{
    private readonly IMediator _mediator;

    public GetUserProfileEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/users/{UserId:Guid}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetUserProfileRequest req, CancellationToken ct)
    {
        var result = await _mediator.Send(req.Adapt<GetUserProfileQuery>(), ct);
        await result.On(success => SendAsync(success.Adapt<GetUserProfileResponse>(), cancellation: ct), errors =>
        {
            foreach (var error in errors)
                AddError(error.Message, error.ErrorCode, (Severity)error.Severity);

            return SendErrorsAsync(cancellation: ct);
        });
    }
}

public record GetUserProfileRequest
{
    public Guid UserId { get; init; }
}

public record struct GetUserProfileResponse(Guid Id, string UserName, string DisplayName, DateTimeOffset MemberSince, IEnumerable<UserTweetDto> Tweets);