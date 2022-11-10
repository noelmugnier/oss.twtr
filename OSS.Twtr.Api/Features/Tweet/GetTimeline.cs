using OSS.Twtr.App.Application;
using OSS.Twtr.Application;
using OSS.Twtr.Common.Infrastructure;

namespace OSS.Twtr.Api.Features;

public record GetTimelineRequest
{
    public string? ContinuationToken { get; init; }
}

public record GetTimelineResponse
{
    public string ContinuationToken { get; init; }
    public IEnumerable<TweetDto> Tweets { get; init; }
}

public sealed class GetTimelineEndpoint : TwtrEndpoint<GetTimelineRequest, GetTimelineResponse>
{
    private readonly IQueryDispatcher _mediator;
    public GetTimelineEndpoint(IQueryDispatcher mediator) => _mediator = mediator;

    public override void Configure()
    {
        Get("/timeline");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetTimelineRequest req, CancellationToken ct)
    {
        var cmdResult = await _mediator.Execute(new GetTimelineQuery(req.ContinuationToken), ct);
        await cmdResult.On(result => SendAsync(new GetTimelineResponse
            {
                Tweets = result.Tweets,
                ContinuationToken = result.ContinuationToken
            }, cancellation: ct),
            errors => SendResultErrorsAsync(errors, ct));
    }
}