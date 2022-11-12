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

public record struct RetweetCommand(Guid UserId, Guid TweetId) : ICommand<Result<Unit>>;

public sealed class RetweetValidator : AbstractValidator<RetweetCommand>
{
    public RetweetValidator()
    {
        RuleFor(x => x.UserId).NotEqual(Guid.Empty)
            .WithMessage("You must be authenticated to retweet");

        RuleFor(x => x.TweetId).NotEqual(Guid.Empty)
            .WithMessage("You must specify the tweet to retweet");
    }
}

internal sealed class RetweetHandler : ICommandHandler<RetweetCommand, Result<Unit>>
{
    private readonly AppDbContext _repository;
    public RetweetHandler(AppDbContext repository) => _repository = repository;

    public async Task<Result<Unit>> Handle(RetweetCommand request, CancellationToken ct)
    {
        var existingRetweet = await _repository.Set<Tweet>().SingleOrDefaultAsync(c => 
            c.Kind == TweetKind.Retweet 
            && c.ReferenceTweetId == TweetId.From(request.TweetId)
            && c.AuthorId == UserId.From(request.UserId), ct);

        if (existingRetweet != null)
            return new Result<Unit>(new Error("Cannot retweet the same tweet twice"));
        
        var tweet = await _repository.Set<Tweet>().SingleOrDefaultAsync(c => c.Id == TweetId.From(request.TweetId), ct);
        if(tweet == null)
            return new Result<Unit>(new Error("Tweet not found"));
        
        var block = await _repository.Set<Block>().SingleOrDefaultAsync(c => c.UserId == tweet.AuthorId && c.UserIdToBlock == UserId.From(request.UserId), ct);
        if (block != null)
            return new Result<Unit>(new Error("Cannot retweet this tweet, author blocked you"));
        
        var retweet = tweet.Retweet(UserId.From(request.UserId));
        await _repository.AddAsync(retweet, ct);    

        tweet.RetweetsCount++;
        
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
                            proposedValues[property] = (int)databaseValue + 1;
                    }

                    // Refresh original values to bypass next concurrency check
                    entry.OriginalValues.SetValues(databaseValues);
                }
            }
        }
        
        return results > 0 ? new Result<Unit>(Unit.Value):new Result<Unit>(new Error("Failed to retweet tweet"));
    }
}