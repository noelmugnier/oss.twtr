using FluentValidation;
using Mapster;
using OSS.Twtr.App.Application;
using OSS.Twtr.Application;
using OSS.Twtr.Common.Infrastructure;

namespace OSS.Twtr.Api.Features;

public record GetTweetRepliesRequest
{
    public Guid TweetId { get; init; }
    public string? ContinuationToken { get; init; }
}

public record GetTweetRepliesResponse
{
    public string ContinuationToken { get; init; }
    public IEnumerable<TweetDto> Tweets { get; init; }
}

public sealed class GetTweetRepliesRequestValidator : AbstractValidator<GetTweetRepliesRequest>
{
    public GetTweetRepliesRequestValidator()
    {
        RuleFor(x => x.TweetId).NotEqual(Guid.Empty);
    }
}

public sealed class GetTweetRepliesEndpoint : TwtrEndpoint<GetTweetRepliesRequest, GetTweetRepliesResponse>
{
    private readonly IQueryDispatcher _mediator;
    public GetTweetRepliesEndpoint(IQueryDispatcher mediator) => _mediator = mediator;

    public override void Configure()
    {
        Get("/tweets/{TweetId:Guid}/replies");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetTweetRepliesRequest req, CancellationToken ct)
    {
        var cmdResult = await _mediator.Execute(req.Adapt<GetTweetRepliesQuery>(), ct);
        await cmdResult.On(result => SendAsync(new GetTweetRepliesResponse
            {
                Tweets = result.Tweets,
                ContinuationToken = result.ContinuationToken
            }, cancellation: ct),
            errors => SendResultErrorsAsync(errors, ct));
    }
}