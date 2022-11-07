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

public record struct QuoteTweetCommand(Guid UserId, Guid TweetId, string? Message, TweetAllowedReplies AllowedReplies) : ICommand<Result<Unit>>;

internal sealed class QuoteTweetValidator : AbstractValidator<QuoteTweetCommand>
{
    public QuoteTweetValidator()
    {
        RuleFor(x => x.TweetId).NotEqual(Guid.Empty)
            .WithMessage("You must specify a tweet to quote");
        
        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Quoting a tweet require a message")
            .MaximumLength(140).WithMessage("Quote is limited to 140 characters");
        
        RuleFor(x => x.UserId).NotEqual(Guid.Empty)
            .WithMessage("You must be authenticated to quote a tweet");
    }
}

internal sealed class QuoteTweetHandler : ICommandHandler<QuoteTweetCommand, Result<Unit>>
{
    private readonly AppDbContext _repository;
    public QuoteTweetHandler(AppDbContext repository) => _repository = repository;

    public async Task<Result<Unit>> Handle(QuoteTweetCommand request, CancellationToken ct)
    {
        var tweet = await _repository.Set<Tweet>().SingleAsync(c => c.Id == TweetId.From(request.TweetId), ct);
        var quote = tweet.Quote(request.Message, UserId.From(request.UserId), request.AllowedReplies);
        
        await _repository.AddAsync(quote, ct);    

        var results= await _repository.SaveChangesAsync(ct);
        return results > 0 ? new Result<Unit>(Unit.Value):new Result<Unit>(new Error("Failed to quote tweet"));
    }
}