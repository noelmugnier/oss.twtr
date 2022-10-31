using System.Security.Claims;
using OSS.Twtr.Common.Application;
using OSS.Twtr.Common.Infrastructure;
using OSS.Twtr.Tweet.Application;

namespace OSS.Twtr.Tweet.Infrastructure;

public record CreateTweetRequest
{
    public string Message { get; init; }
}

public sealed class CreateTweetEndpoint : TwtrEndpoint<CreateTweetRequest, Guid>
{
    private readonly ICommandDispatcher _mediator;
    public CreateTweetEndpoint(ICommandDispatcher mediator) => _mediator = mediator;

    public override void Configure()
    {
        Post("/tweets");
        Policies("auth");
    }

    public override async Task HandleAsync(CreateTweetRequest req, CancellationToken ct)
    {
        var result = await _mediator.Execute(
            new CreateTweetCommand(Guid.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value),
                req.Message), ct);

        await result.On(
            tweetId => SendCreatedAtAsync<GetTweetEndpoint>(new {TweetId = tweetId}, tweetId, cancellation: ct),
            errors => SendResultErrorsAsync(errors, ct));
    }
}