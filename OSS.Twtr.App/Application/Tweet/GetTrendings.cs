using Microsoft.EntityFrameworkCore;
using OSS.Twtr.App.Infrastructure;
using OSS.Twtr.Application;
using OSS.Twtr.Core;

namespace OSS.Twtr.App.Application;

public record struct GetTrendingsQuery() : IQuery<Result<IEnumerable<TrendingDto>>>;

public record struct TrendingDto
{
    public string Name { get; set; }
    public int TweetCount { get; set; }
}

internal sealed class GetTrendingsHandler : IQueryHandler<GetTrendingsQuery, Result<IEnumerable<TrendingDto>>>
{
    private readonly ReadDbContext _db;
    public GetTrendingsHandler(ReadDbContext db) => _db = db;

    public async Task<Result<IEnumerable<TrendingDto>>> Handle(GetTrendingsQuery request, CancellationToken ct)
    {
        try
        {
            var now = DateTime.UtcNow.AddDays(-10);
            const int take = 10;
            const int minTweetsCountToBeTrending = 5;

            var trends = await _db.Set<ReadOnlyTrend>()
                .Where(x => x.AnalyzedOn > now)
                .GroupBy(x => x.Name)
                .Where(x => x.Sum(t => t.TweetCount) > minTweetsCountToBeTrending)
                .OrderByDescending(x => x.Sum(t => t.TweetCount))
                .Select(x => new TrendingDto
                {
                    Name = x.Key,
                    TweetCount = x.Sum(t => t.TweetCount)
                })
                .Take(take)
                .ToListAsync(ct);

            return new Result<IEnumerable<TrendingDto>>(trends);
        }
        catch (Exception e)
        {
            return new Result<IEnumerable<TrendingDto>>(e);
        }
    }
}