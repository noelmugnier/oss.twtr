using OSS.Twtr.App.Application;
using OSS.Twtr.Application;
using OSS.Twtr.Common.Infrastructure;

namespace OSS.Twtr.Api.Features;

public record GetTrendingsRequest
{
}

public record GetTrendingsResponse
{
    public IEnumerable<TrendingDto> Trends { get; init; }
}

public sealed class GetTrendingsEndpoint : TwtrEndpoint<GetTrendingsRequest, IEnumerable<TrendingDto>>
{
    private readonly IQueryDispatcher _mediator;
    public GetTrendingsEndpoint(IQueryDispatcher mediator) => _mediator = mediator;

    public override void Configure()
    {
        Get("/trendings");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetTrendingsRequest req, CancellationToken ct)
    {
        var cmdResult = await _mediator.Execute(new GetTrendingsQuery(), ct);
        await cmdResult.On(result => SendAsync(result, cancellation: ct),
            errors => SendResultErrorsAsync(errors, ct));
    }
}