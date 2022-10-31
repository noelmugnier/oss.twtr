using FluentValidation;
using Mapster;
using OSS.Twtr.Common.Application;
using OSS.Twtr.Common.Infrastructure;
using OSS.Twtr.Tweet.Application;

namespace OSS.Twtr.Tweet.Infrastructure;

public record GetTweetRequest
{
    public Guid TweetId { get; init; }
}

public sealed class GetTweetRequestValidator : AbstractValidator<GetTweetRequest>
{
    public GetTweetRequestValidator()
    {
        RuleFor(x => x.TweetId).NotEqual(Guid.Empty);
    }
}

public sealed class GetTweetEndpoint : TwtrEndpoint<GetTweetRequest, TweetDto>
{
    private readonly IQueryDispatcher _mediator;
    public GetTweetEndpoint(IQueryDispatcher mediator) => _mediator = mediator;

    public override void Configure()
    {
        Get("/tweets/{TweetId:Guid}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetTweetRequest req, CancellationToken ct)
    {
        var cmdResult = await _mediator.Execute(req.Adapt<GetTweetQuery>(), ct);
        await cmdResult.On(tweet => SendAsync(tweet, cancellation: ct),
            errors => SendResultErrorsAsync(errors, ct));
    }
}