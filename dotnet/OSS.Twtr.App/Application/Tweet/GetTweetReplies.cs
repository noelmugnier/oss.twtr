using FluentValidation;
using LinqToDB;
using LinqToDB.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OSS.Twtr.App.Domain.Enums;
using OSS.Twtr.App.Infrastructure;
using OSS.Twtr.Application;
using OSS.Twtr.Core;

namespace OSS.Twtr.App.Application;

public record struct GetTweetRepliesQuery(Guid? UserId, Guid TweetId, string? ContinuationToken) : 
IQuery<Result<TweetsResultDto>>;
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
    private readonly ReadDbContext _db;
    public GetTweetRepliesHandler(IContinuationTokenManager continuationTokenManager, ReadDbContext db)
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

            IQueryable<TweetDto> tweetQuery = null;

            if (request.UserId.HasValue)
                tweetQuery =
                    from c in _db.Set<ReadOnlyTweet>()
                    join a in _db.Set<ReadOnlyUser>() on c.AuthorId equals a.Id
                    from l in _db.Set<ReadOnlyLike>()
                        .LeftJoin(s => s.UserId == request.UserId.Value && s.TweetId == c.Id)
                    from r in _db.Set<ReadOnlyTweet>()
                        .LeftJoin(s => s.ReferenceTweetId == c.Id && s.Kind == TweetKind.Retweet && s.AuthorId ==
                            request.UserId.Value)
                    where c.ReferenceTweetId == request.TweetId
                    orderby c.PostedOn
                    select new TweetDto(c.Id, c.Kind, c.Message, c.PostedOn, new AuthorDto(c.Author.Id, c.Author
                        .UserName, c.Author.DisplayName), c.ReferenceTweet != null
                        ? new ReferenceTweetDto(c.ReferenceTweet.Id,
                            c.ReferenceTweet.Kind, c.ReferenceTweet.Message, c.ReferenceTweet.PostedOn,
                            new AuthorDto(c.ReferenceTweet.Author.Id, c.ReferenceTweet.Author.UserName,
                                c.ReferenceTweet.Author.DisplayName))
                        : null, c.ThreadId, l != null, r != null, c.LikesCount, c.RetweetsCount);
            else
                tweetQuery =
                    from c in _db.Set<ReadOnlyTweet>()
                    join a in _db.Set<ReadOnlyUser>() on c.AuthorId equals a.Id
                    where c.ReferenceTweetId == request.TweetId
                    orderby c.PostedOn
                    select new TweetDto(c.Id, c.Kind, c.Message, c.PostedOn, new AuthorDto(c.Author.Id, c.Author
                        .UserName, c.Author.DisplayName), c.ReferenceTweet != null
                        ? new ReferenceTweetDto(c.ReferenceTweet.Id,
                            c.ReferenceTweet.Kind, c.ReferenceTweet.Message, c.ReferenceTweet.PostedOn, new AuthorDto(c
                                .ReferenceTweet.Author.Id, c.ReferenceTweet.Author.UserName, c.ReferenceTweet.Author
                                .DisplayName))
                        : null, c.ThreadId, false, false, c.LikesCount, c.RetweetsCount);

            var replies = await tweetQuery.Skip(skip).Take(take).ToListAsyncLinqToDB(ct);
                
            skip += take;

            return new Result<TweetsResultDto>(new TweetsResultDto(replies, _continuationTokenManager
            .CreateContinuationToken(now, skip, replies.Count == take)));
        }
        catch (Exception e)
        {
            return new Result<TweetsResultDto>(e);
        }
    }
}