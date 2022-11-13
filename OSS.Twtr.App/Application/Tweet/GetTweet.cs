using FluentValidation;
using LinqToDB;
using LinqToDB.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OSS.Twtr.App.Domain.Enums;
using OSS.Twtr.App.Infrastructure;
using OSS.Twtr.Application;
using OSS.Twtr.Core;

namespace OSS.Twtr.App.Application;

public record struct GetTweetQuery(Guid? UserId, Guid TweetId) : IQuery<Result<TweetDto>>;
public sealed class GetTweetValidator : AbstractValidator<GetTweetQuery>
{
    public GetTweetValidator()
    {
        RuleFor(x => x.TweetId).NotEqual(Guid.Empty)
            .WithMessage("You must specify a tweet id");
    }
}
public record struct TweetDto(Guid Id, TweetKind Kind, string? Message, DateTime PostedOn, AuthorDto Author, 
ReferenceTweetDto? ReferenceTweet, Guid? ThreadId, bool Liked, bool Retweeted, int LikesCount, int RetweetsCount);
public record struct ReferenceTweetDto(Guid Id, TweetKind Kind, string? Message, DateTime PostedOn, AuthorDto Author);
public record struct AuthorDto(Guid Id, string UserName, string? DisplayName);

internal sealed class GetTweetHandler : IQueryHandler<GetTweetQuery, Result<TweetDto>>
{
    private readonly ReadDbContext _db;
    public GetTweetHandler(ReadDbContext db) => _db = db;

    public async Task<Result<TweetDto>> Handle(GetTweetQuery request, CancellationToken ct)
    {
        try
        {
            IQueryable<TweetDto> tweetQuery = null;
            
            if(request.UserId.HasValue)
                tweetQuery = 
                    from c in _db.Set<ReadOnlyTweet>()
                    join a in _db.Set<ReadOnlyUser>() on c.AuthorId equals a.Id
                    from l in _db.Set<ReadOnlyLike>()
                        .LeftJoin(s => s.UserId == request.UserId.Value && s.TweetId == c.Id)
                    from r in _db.Set<ReadOnlyTweet>()
                        .LeftJoin(s => s.ReferenceTweetId == c.Id && s.Kind == TweetKind.Retweet && s.AuthorId ==
                            request.UserId.Value)
                    where c.Id == request.TweetId
                    orderby c.PostedOn descending
                    select new TweetDto(c.Id, c.Kind, c.Message, c.PostedOn, new AuthorDto(c.Author.Id, c.Author
                        .UserName, c.Author.DisplayName), c.ReferenceTweet != null ? new ReferenceTweetDto(c.ReferenceTweet.Id, 
                        c.ReferenceTweet.Kind, c.ReferenceTweet.Message, c.ReferenceTweet.PostedOn, new AuthorDto(c.ReferenceTweet.Author.Id, c.ReferenceTweet.Author.UserName, c.ReferenceTweet.Author.DisplayName)) : null, c.ThreadId, l != null, r != null, c.LikesCount, c.RetweetsCount);
            else
                tweetQuery = 
                    from c in _db.Set<ReadOnlyTweet>()
                    join a in _db.Set<ReadOnlyUser>() on c.AuthorId equals a.Id
                    where c.Id == request.TweetId
                    orderby c.PostedOn descending
                    select new TweetDto(c.Id, c.Kind, c.Message, c.PostedOn, new AuthorDto(c.Author.Id, c.Author
                        .UserName, c.Author.DisplayName), c.ReferenceTweet != null ? new ReferenceTweetDto(c.ReferenceTweet.Id, 
                        c.ReferenceTweet.Kind, c.ReferenceTweet.Message, c.ReferenceTweet.PostedOn, new AuthorDto(c
                        .ReferenceTweet.Author.Id, c.ReferenceTweet.Author.UserName, c.ReferenceTweet.Author.DisplayName)
                        ) : null, c.ThreadId, false, false, c.LikesCount, c.RetweetsCount);

            var tweet = await tweetQuery.SingleOrDefaultAsyncLinqToDB(ct);
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