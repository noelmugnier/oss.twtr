using System.Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OSS.Twtr.App.Domain.Entities;
using OSS.Twtr.App.Domain.Repositories;
using OSS.Twtr.App.Domain.ValueObjects;
using OSS.Twtr.App.Infrastructure;
using OSS.Twtr.Application;
using OSS.Twtr.Core;

namespace OSS.Twtr.App.Application;

public record struct LikeTweetCommand(Guid UserId, Guid TweetId) : ICommand<Result<Unit>>;

public sealed class LikeTweetValidator : AbstractValidator<LikeTweetCommand>
{
    public LikeTweetValidator()
    {
        RuleFor(x => x.UserId).NotEqual(Guid.Empty)
            .WithMessage("You must be authenticated to like a tweet");

        RuleFor(x => x.TweetId).NotEqual(Guid.Empty)
            .WithMessage("You must specify the tweet to like");
    }
}

internal sealed class LikeTweetHandler : ICommandHandler<LikeTweetCommand, Result<Unit>>
{
    private readonly AppDbContext _repository;
    public LikeTweetHandler(AppDbContext repository) => _repository = repository;

    public async Task<Result<Unit>> Handle(LikeTweetCommand request, CancellationToken ct)
    {
        var tweet = await _repository.Set<Tweet>().SingleOrDefaultAsync(t => t.Id == TweetId.From(request.TweetId), ct);
        if (tweet == null)
            return new Result<Unit>(new Error("Tweet not found"));

        var like = Like.Create(UserId.From(request.UserId), tweet.Id);
        await _repository.AddAsync(like, ct);

        tweet.LikesCount++;
        
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
                            proposedValues[property] = (int)databaseValue + 1;
                    }

                    // Refresh original values to bypass next concurrency check
                    entry.OriginalValues.SetValues(databaseValues);
                }
            }
        }
        
        return results > 0 ? new Result<Unit>(Unit.Value):new Result<Unit>(new Error("Failed to like tweet"));
    }
}