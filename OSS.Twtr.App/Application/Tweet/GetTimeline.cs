using Microsoft.EntityFrameworkCore;
using OSS.Twtr.App.Domain.Enums;
using OSS.Twtr.App.Infrastructure;
using OSS.Twtr.Application;
using OSS.Twtr.Core;

namespace OSS.Twtr.App.Application;

public record struct GetTimelineQuery(Guid? UserId, string? ContinuationToken) : IQuery<Result<TweetsResultDto>>;

public record struct TweetsResultDto
{
    public TweetsResultDto(List<TweetDto> tweets, string? continuationToken)
    {
        Tweets = tweets;
        ContinuationToken = Tweets != null && Tweets.Any() ? continuationToken : null;
    }
    
    public string ContinuationToken { get;  }
    public List<TweetDto> Tweets { get; }
}

internal sealed class GetTimelineHandler : IQueryHandler<GetTimelineQuery, Result<TweetsResultDto>>
{
    private readonly IContinuationTokenManager _continuationTokenManager;
    private readonly IReadDbContext _db;
    public GetTimelineHandler(IContinuationTokenManager continuationTokenManager, IReadDbContext db)
    {
        _continuationTokenManager = continuationTokenManager;
        _db = db;
    }

    public async Task<Result<TweetsResultDto>> Handle(GetTimelineQuery request, CancellationToken ct)
    {
        try
        {
            var (now, skip) = _continuationTokenManager.ReadContinuationToken(request.ContinuationToken);
            const int take = 20;
            
            var tweets = await _db.Get<ReadOnlyTweet>()
                .Where(t => t.PostedOn <= now && t.Kind != TweetKind.Reply)
                .OrderByDescending(t=> t.PostedOn)
                .Select(c => new TweetDto(c.Id, c.Kind, c.Message, c.PostedOn, new AuthorDto(c.Author.Id, c.Author
                    .UserName, c.Author.DisplayName ?? c.Author.UserName), c.ReferenceTweet != null ? new ReferenceTweetDto(c
                    .ReferenceTweet.Id, 
                    c.ReferenceTweet.Kind, c.ReferenceTweet.Message, c.ReferenceTweet.PostedOn, 
                    new AuthorDto(c
                    .ReferenceTweet.Author.Id, c.ReferenceTweet.Author.UserName, c.ReferenceTweet.Author.DisplayName 
                    ?? c.ReferenceTweet.Author.UserName)) : null, c.ThreadId))
                .Skip(skip)
                .Take(take)
                .ToListAsync(ct);

            skip += take;

            return new Result<TweetsResultDto>(new TweetsResultDto(tweets, _continuationTokenManager
                .CreateContinuationToken(now, skip)));
        }
        catch (Exception e)
        {
            return new Result<TweetsResultDto>(e);
        }
    }
}