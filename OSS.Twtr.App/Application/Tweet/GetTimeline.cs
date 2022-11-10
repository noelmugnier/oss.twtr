using System.Buffers.Text;
using System.Text;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OSS.Twtr.App.Domain.Entities;
using OSS.Twtr.App.Domain.Enums;
using OSS.Twtr.App.Infrastructure;
using OSS.Twtr.Application;
using OSS.Twtr.Core;

namespace OSS.Twtr.App.Application;

public record struct GetTimelineQuery(string? ContinuationToken) : IQuery<Result<TimeLineResultDto>>;

public record struct TimeLineResultDto
{
    public string ContinuationToken { get; set; }
    public List<TweetDto> Tweets { get; set; }
}

internal sealed class GetTimelineHandler : IQueryHandler<GetTimelineQuery, Result<TimeLineResultDto>>
{
    private readonly IReadDbContext _db;
    public GetTimelineHandler(IReadDbContext db) => _db = db;

    public async Task<Result<TimeLineResultDto>> Handle(GetTimelineQuery request, CancellationToken ct)
    {
        try
        {
            var now = DateTime.UtcNow;
            var skip = 0;
            const int take = 20;

            if (!string.IsNullOrWhiteSpace(request.ContinuationToken))
            {
                var tokens = Encoding.UTF8.GetString(Convert.FromBase64String(request.ContinuationToken)).Split("_");
                now = DateTime.Parse(tokens[0]);
                skip = int.Parse(tokens[1]);
            }
            
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

            return new Result<TimeLineResultDto>(new TimeLineResultDto
            {
                Tweets = tweets,
                ContinuationToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{now:s}_{skip}"))
            });
        }
        catch (Exception e)
        {
            return new Result<TimeLineResultDto>(e);
        }
    }
}