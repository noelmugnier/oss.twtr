using Microsoft.EntityFrameworkCore;
using OSS.Twtr.App.Domain.Entities;
using OSS.Twtr.App.Domain.Enums;
using OSS.Twtr.App.Domain.ValueObjects;
using OSS.Twtr.App.Infrastructure;
using OSS.Twtr.Application;
using OSS.Twtr.Core;

namespace OSS.Twtr.App.Application;

public record struct TokenizeTweetCommand(Guid TweetId) : ICommand<Result<Unit>>;

internal sealed class TokenizeTweetHandler : ICommandHandler<TokenizeTweetCommand, Result<Unit>>
{
    private readonly AppDbContext _repository;
    private readonly ITweetTokenizer _tweetTokenizer;

    public TokenizeTweetHandler(AppDbContext repository, ITweetTokenizer tweetTokenizer)
    {
        _repository = repository;
        _tweetTokenizer = tweetTokenizer;
    }

    public async Task<Result<Unit>> Handle(TokenizeTweetCommand request, CancellationToken ct)
    {
        var tweet = await _repository.Set<Tweet>()
            .Where(t => t.Id == TweetId.From(request.TweetId))
            .SingleOrDefaultAsync(ct);

        if (tweet == null)
            return new Result<Unit>(Unit.Value);

        var tokens = new List<string>();
        if (tweet.Kind == TweetKind.Retweet)
        {
            tokens = await _repository
                .Set<Token>()
                .Where(t => t.TweetId == tweet.ReferenceTweetId)
                .Select(c => c.Value)
                .ToListAsync(ct);
        }
        else
        {
            if (string.IsNullOrWhiteSpace(tweet.Message))
                return new Result<Unit>(Unit.Value);

            tokens = _tweetTokenizer.TokenizeMessage(tweet.Message).ToList();
        }
        
        if(!tokens.Any())
            return new Result<Unit>(Unit.Value);

        foreach (var token in tokens)
            await _repository.Set<Token>().AddAsync(new Token(token, tweet.Id), ct);

        var results = await _repository.SaveChangesAsync(ct);
        return results > 0
            ? new Result<Unit>(Unit.Value)
            : new Result<Unit>(new Error("Failed to tokenize tweet message"));
    }
}