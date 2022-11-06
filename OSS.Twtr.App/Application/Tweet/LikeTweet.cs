using FluentValidation;
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
        var like = Like.Create(UserId.From(request.UserId), TweetId.From(request.TweetId));
        await _repository.AddAsync(like, ct);
        
        var results= await _repository.SaveChangesAsync(ct);
        return results > 0 ? new Result<Unit>(Unit.Value):new Result<Unit>(new Error("Failed to like tweet"));
    }
}