using System.Security.Claims;
using OSS.Twtr.App.Application;
using OSS.Twtr.App.Domain.Enums;
using OSS.Twtr.Application;
using OSS.Twtr.Common.Infrastructure;

namespace OSS.Twtr.Api.Features;

public record QuoteTweetRequest
{
    public Guid TweetId { get; init; }
    public string Message { get; init; }
    public TweetAllowedReplies AllowedReplies { get; init; }
}

public sealed class QuoteTweetEndpoint : TwtrEndpoint<QuoteTweetRequest>
{
    private readonly ICommandDispatcher _mediator;
    public QuoteTweetEndpoint(ICommandDispatcher mediator) => _mediator = mediator;

    public override void Configure()
    {
        Post("/tweets/{TweetId:Guid}/quote");
        Policies("auth");
    }

    public override async Task HandleAsync(QuoteTweetRequest req, CancellationToken ct)
    {
        var result = await _mediator.Execute(
            new QuoteTweetCommand(Guid.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value),
                req.TweetId, req.Message, req.AllowedReplies), ct);

        await result.On(
            tweetId => SendOkAsync(cancellation: ct),
            errors => SendResultErrorsAsync(errors, ct));
    }
}