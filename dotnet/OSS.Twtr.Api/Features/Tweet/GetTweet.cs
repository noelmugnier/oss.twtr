using System.Security.Claims;
using FluentValidation;
using OSS.Twtr.App.Application;
using OSS.Twtr.Application;
using OSS.Twtr.Common.Infrastructure;

namespace OSS.Twtr.Api.Features;

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
        var cmdResult = await _mediator.Execute(new GetTweetQuery(User.Identity.IsAuthenticated
            ? Guid.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value)
            : null, req.TweetId), ct);
        
        await cmdResult.On(tweet => SendAsync(tweet, cancellation: ct),
            errors => SendResultErrorsAsync(errors, ct));
    }
}