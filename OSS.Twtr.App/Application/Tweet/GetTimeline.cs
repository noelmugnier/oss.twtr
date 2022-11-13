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
                        join a in _db.Set<ReadOnlyUser>() on c.AuthorId equals a.Id
                        from f in _db.Set<ReadOnlySubscription>()
                            .LeftJoin(s => s.FollowerUserId == request.UserId.Value && s.SubscribedToUserId == a.Id)
                        from l in _db.Set<ReadOnlyLike>()
                            .LeftJoin(s => s.UserId == request.UserId.Value && s.TweetId == c.Id)
                        from r in _db.Set<ReadOnlyTweet>()
                            .LeftJoin(s => s.ReferenceTweetId == c.Id && s.Kind == TweetKind.Retweet && s.AuthorId == request.UserId.Value)
                        from m in _db.Set<ReadOnlyMute>()
                            .LeftJoin(s => s.UserId == request.UserId.Value && s.UserIdToMute == a.Id)
                        from b in _db.Set<ReadOnlyBlock>()
                            .LeftJoin(s => s.UserId == a.Id && s.UserIdToBlock == request.UserId.Value)
                        where ((f != null && m == null && b == null) || c.AuthorId == request.UserId.Value) && c.PostedOn <= now && c.Kind != TweetKind.Reply
                        orderby c.PostedOn descending
                        select new TweetDto(c.Id, c.Kind, c.Message, c.PostedOn, new AuthorDto(c.Author.Id, c.Author
                            .UserName, c.Author.DisplayName), c.ReferenceTweet != null ? new ReferenceTweetDto(c.ReferenceTweet.Id, 
                            c.ReferenceTweet.Kind, c.ReferenceTweet.Message, c.ReferenceTweet.PostedOn, new AuthorDto
                            (c.ReferenceTweet.Author.Id, c.ReferenceTweet.Author.UserName, c.ReferenceTweet.Author
                            .DisplayName)) : null, c.ThreadId, l != null, r != null, c.LikesCount, c.RetweetsCount);
            else
                tweets = from c in _db.Set<ReadOnlyTweet>()
                    join a in _db.Set<ReadOnlyUser>() on c.AuthorId equals a.Id
                    from l in _db.Set<ReadOnlyLike>()
                        .LeftJoin(s => s.UserId == request.UserId.Value && s.TweetId == c.Id)
                    from r in _db.Set<ReadOnlyTweet>()
                        .LeftJoin(s => s.ReferenceTweetId == c.Id && s.Kind == TweetKind.Retweet && s.AuthorId ==
                            request.UserId.Value)
                    where c.PostedOn <= now && c.Kind != TweetKind.Reply
                    orderby c.PostedOn descending
                    select new TweetDto(c.Id, c.Kind, c.Message, c.PostedOn, new AuthorDto(c.Author.Id, c.Author
                        .UserName, c.Author.DisplayName), c.ReferenceTweet != null ? new ReferenceTweetDto(c.ReferenceTweet.Id, 
                        c.ReferenceTweet.Kind, c.ReferenceTweet.Message, c.ReferenceTweet.PostedOn, new AuthorDto(c
                        .ReferenceTweet.Author.Id, c.ReferenceTweet.Author.UserName, c.ReferenceTweet.Author
                        .DisplayName)) : null, c.ThreadId, l != null, r != null, c.LikesCount, c.RetweetsCount);

            var results = await tweets.Skip(skip).Take(take).ToListAsyncLinqToDB(ct);

            skip += take;

            return new Result<TweetsResultDto>(new TweetsResultDto(results, _continuationTokenManager
                .CreateContinuationToken(now, skip, results.Count == take)));
        }
        catch (Exception e)
        {
            return new Result<TweetsResultDto>(e);
        }
    }
}