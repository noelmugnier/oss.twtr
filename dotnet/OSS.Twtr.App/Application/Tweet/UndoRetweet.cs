using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OSS.Twtr.App.Domain.Entities;
using OSS.Twtr.App.Domain.Enums;
using OSS.Twtr.App.Domain.Repositories;
using OSS.Twtr.App.Domain.ValueObjects;
using OSS.Twtr.App.Infrastructure;
using OSS.Twtr.Application;
using OSS.Twtr.Core;

namespace OSS.Twtr.App.Application;

public record struct UndoRetweetCommand(Guid UserId, Guid TweetId) : ICommand<Result<Unit>>;

public sealed class UndoRetweetValidator : AbstractValidator<UndoRetweetCommand>
{
    public UndoRetweetValidator()
    {
        RuleFor(x => x.UserId).NotEqual(Guid.Empty)
            .WithMessage("You must be authenticated to undo retweet");

        RuleFor(x => x.TweetId).NotEqual(Guid.Empty)
            .WithMessage("You must specify the tweet to undo retweet");
    }
}

internal sealed class UndoRetweetHandler : ICommandHandler<UndoRetweetCommand, Result<Unit>>
{
    private readonly AppDbContext _repository;
    public UndoRetweetHandler(AppDbContext repository) => _repository = repository;

    public async Task<Result<Unit>> Handle(UndoRetweetCommand request, CancellationToken ct)
    {
        var tweet = await _repository.Set<Tweet>().SingleOrDefaultAsync(c =>
            c.Kind == TweetKind.Retweet
            && c.ReferenceTweetId == TweetId.From(request.TweetId)
            && c.AuthorId == UserId.From(request.UserId), ct);

        if (tweet == null)
            return new Result<Unit>(Unit.Value);
        
        _repository.Remove(tweet);    

        var originalTweet = await _repository.Set<Tweet>().SingleOrDefaultAsync(c => c.Id == tweet.ReferenceTweetId, ct);
        if(originalTweet == null)
            return new Result<Unit>(Unit.Value);
        
        originalTweet.RetweetsCount--;
        
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

                        if(property.Name == nameof(Tweet.RetweetsCount))
                            proposedValues[property] = (int)databaseValue - 1;
                    }

                    // Refresh original values to bypass next concurrency check
                    entry.OriginalValues.SetValues(databaseValues);
                }
            }
        }
        
        return results > 0 ? new Result<Unit>(Unit.Value) : new Result<Unit>(new Error("Failed to undo retweet"));
    }
}