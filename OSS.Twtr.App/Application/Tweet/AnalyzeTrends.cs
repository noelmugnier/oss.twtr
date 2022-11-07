using Microsoft.EntityFrameworkCore;
using OSS.Twtr.App.Domain.Entities;
using OSS.Twtr.App.Infrastructure;
using OSS.Twtr.Application;
using OSS.Twtr.Core;

namespace OSS.Twtr.App.Application;

public record struct AnalyzeTrendsCommand(DateTime ToDate) : ICommand<Result<Unit>>;

internal sealed class AnalyzeTrendsHandler : ICommandHandler<AnalyzeTrendsCommand, Result<Unit>>
{
    private readonly AppDbContext _repository;
    public AnalyzeTrendsHandler(AppDbContext repository) => _repository = repository;

    public async Task<Result<Unit>> Handle(AnalyzeTrendsCommand request, CancellationToken ct)
    {
        var lastAnalyzeDate = await _repository.Set<Trending>().MaxAsync(c => (DateTime?)c.AnalyzedOn, ct);
        if(!lastAnalyzeDate.HasValue)
            lastAnalyzeDate = DateTime.MinValue;

        var tokens = await _repository.Set<Token>()
            .Where(t => t.CreatedOn > lastAnalyzeDate && t.CreatedOn <= request.ToDate)
            .GroupBy(t => t.Value, token => token.TweetId)
            .Select(t => new { Name = t.Key, TweetCount = t.Count()})
            .ToListAsync(ct);

        foreach (var token in tokens)
        {
            var trending = new Trending(token.Name, token.TweetCount, request.ToDate);
            await _repository.Set<Trending>().AddAsync(trending, ct);
        }
        
        var results= await _repository.SaveChangesAsync(ct);
        return results > 0 ? new Result<Unit>(Unit.Value):new Result<Unit>(new Error("Failed to analyze trends"));
    }
}