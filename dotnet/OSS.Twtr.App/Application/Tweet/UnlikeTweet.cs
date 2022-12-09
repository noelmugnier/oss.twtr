using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OSS.Twtr.App.Domain.Entities;
using OSS.Twtr.App.Domain.Repositories;
using OSS.Twtr.App.Domain.ValueObjects;
using OSS.Twtr.App.Infrastructure;
using OSS.Twtr.Application;
using OSS.Twtr.Core;

namespace OSS.Twtr.App.Application;

public record struct UnlikeTweetCommand(Guid UserId, Guid TweetId) : ICommand<Result<Unit>>;

public sealed class UnlikeTweetValidator : AbstractValidator<UnlikeTweetCommand>
{
    public UnlikeTweetValidator()
    {
        RuleFor(x => x.UserId).NotEqual(Guid.Empty)
            .WithMessage("You must be authenticated to unlike a Tweet");

        RuleFor(x => x.TweetId).NotEqual(Guid.Empty)
            .WithMessage("You must specify the tweet to unlike");
    }
}

internal sealed class UnlikeTweetHandler : ICommandHandler<UnlikeTweetCommand, Result<Unit>>
{
    private readonly AppDbContext _repository;
    public UnlikeTweetHandler(AppDbContext repository) => _repository = repository;

    public async Task<Result<Unit>> Handle(UnlikeTweetCommand request, CancellationToken ct)
    {
        var tweet = await _repository.Set<Tweet>().SingleOrDefaultAsync(t => t.Id == TweetId.From(request.TweetId), ct);
        if (tweet == null)
            return new Result<Unit>(new Error("Tweet not found"));

        var like = await _repository.Set<Like>().SingleOrDefaultAsync(b => 
            b.UserId == UserId.From(request.UserId) && b.TweetId == TweetId.From(request.TweetId), ct);

        if (like == null)
            return new Result<Unit>(Unit.Value);
        
        _repository.Remove(like);

        tweet.LikesCount--;
        
        var results = 0;
        while(results == 0)
        {
            try
            {
                results = await _repository.SaveChangesAsync(ct);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                foreach (var entry in ex.Entries)
                {
                    if (entry.Entity is not Tweet) continue;
                    
                    var proposedValues = entry.CurrentValues;
                    var databaseValues = await entry.GetDatabaseValuesAsync(ct);

                    foreach (var property in proposedValues.Properties)
                    {
                        var proposedValue = proposedValues[property];
                        var databaseValue = databaseValues[property];

                        if(property.Name == nameof(Tweet.LikesCount))
                            proposedValues[property] = (int)databaseValue - 1;
                    }

                    // Refresh original values to bypass next concurrency check
                    entry.OriginalValues.SetValues(databaseValues);
                }
            }
        }
        
        return results > 0 ? new Result<Unit>(Unit.Value):new Result<Unit>(new Error("Failed to unlike tweet"));
    }
}