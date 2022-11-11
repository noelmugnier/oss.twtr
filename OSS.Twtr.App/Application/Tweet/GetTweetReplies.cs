using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OSS.Twtr.App.Domain.Enums;
using OSS.Twtr.App.Infrastructure;
using OSS.Twtr.Application;
using OSS.Twtr.Core;

namespace OSS.Twtr.App.Application;

public record struct GetTweetRepliesQuery(Guid TweetId, string? ContinuationToken) : IQuery<Result<TweetsResultDto>>;
public sealed class GetTweetRepliesValidator : AbstractValidator<GetTweetRepliesQuery>
{
    public GetTweetRepliesValidator()
    {
        RuleFor(x => x.TweetId).NotEqual(Guid.Empty)
            .WithMessage("You must specify a tweet id");
    }
}

internal sealed class GetTweetRepliesHandler : IQueryHandler<GetTweetRepliesQuery, Result<TweetsResultDto>>
{
    private readonly IContinuationTokenManager _continuationTokenManager;
    private readonly IReadDbContext _db;
    public GetTweetRepliesHandler(IContinuationTokenManager continuationTokenManager, IReadDbContext db)
    {
        _continuationTokenManager = continuationTokenManager;
        _db = db;
    }

    public async Task<Result<TweetsResultDto>> Handle(GetTweetRepliesQuery request, CancellationToken ct)
    {
        try
        {
            var (now, skip) = _continuationTokenManager.ReadContinuationToken(request.ContinuationToken);
            const int take = 20;
            
            var replies = await _db.Get<ReadOnlyTweet>()
                .Where(t => t.ReferenceTweetId == request.TweetId)
                .OrderBy(t => t.PostedOn)
                .Select(c => 
                    new TweetDto(c.Id, c.Kind, c.Message, c.PostedOn, new AuthorDto(c.Author.Id, c.Author.UserName, c.Author.DisplayName), null, c.ThreadId))
                .Skip(skip)
                .Take(take)
                .ToListAsync(ct);

            skip += take;

            return new Result<TweetsResultDto>(new TweetsResultDto(replies, _continuationTokenManager.CreateContinuationToken(now, skip)));
        }
        catch (Exception e)
        {
            return new Result<TweetsResultDto>(e);
        }
    }
}