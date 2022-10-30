using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OSS.Twtr.Application;
using OSS.Twtr.Core;
using OSS.Twtr.Domain;
using OSS.Twtr.Infrastructure;
using OSS.Twtr.Management.Infrastructure.Endpoints;

namespace OSS.Twtr.Management.Application.Queries;

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
    private readonly IMediator _mediator;
    public GetTweetEndpoint(IMediator mediator) => _mediator = mediator;

    public override void Configure()
    {
        Get("/tweets/{TweetId:Guid}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetTweetRequest req, CancellationToken ct)
    {
        var cmdResult = await _mediator.Send(req.Adapt<GetTweetQuery>(), ct);
        await cmdResult.On(tweet => SendAsync(tweet, cancellation: ct),
            errors => SendResultErrorsAsync(errors, ct));
    }
}

public record struct GetTweetQuery(Guid TweetId) : IQuery<Result<TweetDto>>;

public record struct TweetDto(Guid Id, string Message, DateTime PostedOn);

public record struct AuthorDto(Guid Id, string UserName, string DisplayName);

internal sealed class GetTweetHandler : IQueryHandler<GetTweetQuery, Result<TweetDto>>
{
    private readonly AppDbContext _db;
    public GetTweetHandler(AppDbContext db) => _db = db;

    public async Task<Result<TweetDto>> Handle(GetTweetQuery request, CancellationToken ct)
    {
        var tweet = await _db.Set<Tweet>()
            .SingleOrDefaultAsync(t => t.Id == TweetId.From(request.TweetId), ct);

        return tweet != null
            ? new Result<TweetDto>(new TweetDto(tweet.Id.Value, tweet.Message, tweet.PostedOn))
            : new Result<TweetDto>(new Error($"Tweet {request.TweetId} not found."));
    }
}