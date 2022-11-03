using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OSS.Twtr.App.Infrastructure;
using OSS.Twtr.Application;
using OSS.Twtr.Core;

namespace OSS.Twtr.App.Application;

public record struct GetTweetQuery(Guid TweetId) : IQuery<Result<TweetDto>>;
public sealed class GetTweetValidator : AbstractValidator<GetTweetQuery>
{
    public GetTweetValidator()
    {
        RuleFor(x => x.TweetId).NotEqual(Guid.Empty)
            .WithMessage("You must specify a tweet id");
    }
}
public record struct TweetDto(Guid Id, string Message, DateTime PostedOn, AuthorDto Author);
public record struct AuthorDto(Guid Id, string UserName, string? DisplayName);

internal sealed class GetTweetHandler : IQueryHandler<GetTweetQuery, Result<TweetDto>>
{
    private readonly IReadDbContext _db;
    public GetTweetHandler(IReadDbContext db) => _db = db;

    public async Task<Result<TweetDto>> Handle(GetTweetQuery request, CancellationToken ct)
    {
        try
        {
            var tweet = await _db.Get<ReadOnlyTweet>()
                .Where(t => t.Id == request.TweetId)
                .Select(c => new TweetDto(c.Id, c.Message, c.PostedOn, new AuthorDto(c.Author.Id, c.Author.UserName, c
                    .Author.DisplayName)))
                .SingleOrDefaultAsync(ct);

            return tweet != null
                ? new Result<TweetDto>(tweet)
                : new Result<TweetDto>(new Error($"Tweet {request.TweetId} not found."));
        }
        catch (Exception e)
        {
            return new Result<TweetDto>(e);
        }
    }
}