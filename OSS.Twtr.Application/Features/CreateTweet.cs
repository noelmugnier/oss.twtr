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

public record struct CreateTweetRequest(string Message);

public sealed class CreateTweetEndpoint : TwtrEndpoint<CreateTweetRequest, Guid>
{
    private readonly IMediator _mediator;
    public CreateTweetEndpoint(IMediator mediator) => _mediator = mediator;

    public override void Configure()
    {
        Post("/tweets");
        Policies("auth");
    }

    public override async Task HandleAsync(CreateTweetRequest req, CancellationToken ct)
    {
        var result = await _mediator.Send(
                new CreateTweetCommand(Guid.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value), req.Message), ct);
        
        await result.On(tweetId => SendCreatedAtAsync<GetTweetEndpoint>(new {TweetId = tweetId}, tweetId, cancellation:ct),
            errors => SendResultErrorsAsync(errors, ct));
    }
}

public record struct CreateTweetCommand(Guid UserId, string Message) : ICommand<Result<Guid>>;
public sealed class CreateTweetValidator : AbstractValidator<CreateTweetCommand>
{
    public CreateTweetValidator()
    {
        RuleFor(x => x.Message).NotEmpty();
        RuleFor(x => x.UserId).NotEqual(Guid.Empty);
    }
}

internal sealed class CreateTweetHandler : ICommandHandler<CreateTweetCommand, Result<Guid>>
{
    private readonly AppDbContext _db;
    public CreateTweetHandler(AppDbContext db) => _db = db;

    public async Task<Result<Guid>> Handle(CreateTweetCommand request, CancellationToken ct)
    {
        var user = await _db.Set<User>().SingleOrDefaultAsync(u => u.Id == UserId.From(request.UserId), ct);
        var entry = await _db.Set<Tweet>().AddAsync(Tweet.Create(request.Message, user), ct);
        await _db.SaveChangesAsync(ct);

        return new Result<Guid>(entry.Entity.Id.Value);
    }
}