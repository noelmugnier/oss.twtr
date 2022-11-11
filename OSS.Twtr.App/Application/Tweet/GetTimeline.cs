using LinqToDB;
using LinqToDB.EntityFrameworkCore;
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
    private readonly ReadDbContext _db;
    public GetTimelineHandler(IContinuationTokenManager continuationTokenManager, ReadDbContext db)
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

            IQueryable<TweetDto> tweets = null;
            
            if(request.UserId.HasValue)
                tweets = from c in _db.Set<ReadOnlyTweet>()
                        join a in _db.Set<ReadOnlyAuthor>() on c.AuthorId equals a.Id
                        from f in _db.Set<ReadOnlySubscription>()
                            .LeftJoin(s => s.FollowerUserId == request.UserId.Value && s.SubscribedToUserId == a.Id)
                        where f != null && c.PostedOn <= now && c.Kind != TweetKind.Reply
                        orderby c.PostedOn descending
                        select new TweetDto(c.Id, c.Kind, c.Message, c.PostedOn, new AuthorDto(c.Author.Id, c.Author
                            .UserName, c.Author.DisplayName), c.ReferenceTweet != null ? new ReferenceTweetDto(c.ReferenceTweet.Id, 
                            c.ReferenceTweet.Kind, c.ReferenceTweet.Message, c.ReferenceTweet.PostedOn, new AuthorDto(c.ReferenceTweet.Author.Id, c.ReferenceTweet.Author.UserName, c.ReferenceTweet.Author.DisplayName)) : null, c.ThreadId);
            else
                tweets = from c in _db.Set<ReadOnlyTweet>()
                    join a in _db.Set<ReadOnlyAuthor>() on c.AuthorId equals a.Id
                    where c.PostedOn <= now && c.Kind != TweetKind.Reply
                    orderby c.PostedOn descending
                    select new TweetDto(c.Id, c.Kind, c.Message, c.PostedOn, new AuthorDto(c.Author.Id, c.Author
                        .UserName, c.Author.DisplayName), c.ReferenceTweet != null ? new ReferenceTweetDto(c.ReferenceTweet.Id, 
                        c.ReferenceTweet.Kind, c.ReferenceTweet.Message, c.ReferenceTweet.PostedOn, new AuthorDto(c.ReferenceTweet.Author.Id, c.ReferenceTweet.Author.UserName, c.ReferenceTweet.Author.DisplayName)) : null, c.ThreadId);

            var results = await tweets.Skip(skip).Take(take).ToListAsyncLinqToDB(ct);

            skip += take;

            return new Result<TweetsResultDto>(new TweetsResultDto(results, _continuationTokenManager
                .CreateContinuationToken(now, skip)));
        }
        catch (Exception e)
        {
            return new Result<TweetsResultDto>(e);
        }
    }
}