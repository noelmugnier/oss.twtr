using System.Security.Claims;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OSS.Twtr.Application;
using OSS.Twtr.Core;
using OSS.Twtr.Domain;
using OSS.Twtr.Infrastructure;
using OSS.Twtr.Management.Application.Queries;

namespace OSS.Twtr.Management.Infrastructure.Endpoints;

public record ReplyToTweetRequest
{
    public Guid TweetId { get; set; }
    public string Message { get; init; }
}

public sealed class ReplyToTweetRequestValidator : AbstractValidator<ReplyToTweetRequest>
{
    public ReplyToTweetRequestValidator()
    {
        RuleFor(x => x.Message).NotEmpty();
    }
}

public sealed class ReplyToTweetEndpoint : TwtrEndpoint<ReplyToTweetRequest, Guid>
{
    private readonly IMediator _mediator;
    public ReplyToTweetEndpoint(IMediator mediator) => _mediator = mediator;

    public override void Configure()
    {
        Post("/tweets/{TweetId:Guid}");
        Policies("auth");
    }

    public override async Task HandleAsync(ReplyToTweetRequest req, CancellationToken ct)
    {
        var result = await _mediator.Send(
                new ReplyToTweetCommand(Guid.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value), req.TweetId, req.Message), ct);
        
        await result.On(tweetId => SendCreatedAtAsync<GetTweetEndpoint>(new {TweetId = tweetId}, tweetId, cancellation:ct),
            errors => SendResultErrorsAsync(errors, ct));
    }
}

public record struct ReplyToTweetCommand(Guid UserId, Guid TweetId, string Message) : ICommand<Result<Guid>>;

internal sealed class ReplyToTweetHandler : ICommandHandler<ReplyToTweetCommand, Result<Guid>>
{
    private readonly AppDbContext _db;
    public ReplyToTweetHandler(AppDbContext db) => _db = db;

    public async Task<Result<Guid>> Handle(ReplyToTweetCommand request, CancellationToken ct)
    {
        var user = await _db.Set<User>().SingleOrDefaultAsync(u => u.Id == UserId.From(request.UserId), ct);
        var tweet = await _db.Set<Tweet>().SingleOrDefaultAsync(u => u.Id == TweetId.From(request.TweetId), ct);
        var entry = await _db.Set<Tweet>().AddAsync(tweet.Reply(request.Message, user), ct);
        await _db.SaveChangesAsync(ct);

        return new Result<Guid>(entry.Entity.Id.Value);
    }
}