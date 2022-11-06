using FluentValidation;
using OSS.Twtr.App.Domain.Entities;
using OSS.Twtr.App.Domain.Enums;
using OSS.Twtr.App.Domain.ValueObjects;
using OSS.Twtr.App.Infrastructure;
using OSS.Twtr.Application;
using OSS.Twtr.Core;

namespace OSS.Twtr.App.Application;

public record struct CreateThreadCommand(Guid UserId, IEnumerable<string> Messages, TweetAllowedReplies AllowedReplies) : ICommand<Result<Unit>>;

internal sealed class CreateThreadValidator : AbstractValidator<CreateThreadCommand>
{
    public CreateThreadValidator()
    {
        RuleFor(x => x.Messages).NotNull()
            .ForEach(e => e.NotEmpty().MaximumLength(140));
        RuleFor(x => x.UserId).NotEqual(Guid.Empty)
            .WithMessage("You must be authenticated to perform this action");
    }
}

internal sealed class CreateThreadHandler : ICommandHandler<CreateThreadCommand, Result<Unit>>
{
    private readonly AppDbContext _repository;
    public CreateThreadHandler(AppDbContext repository) => _repository = repository;

    public async Task<Result<Unit>> Handle(CreateThreadCommand request, CancellationToken ct)
    {
        var threadId = ThreadId.New();
        var userId = UserId.From(request.UserId);
        foreach (var message in request.Messages.Reverse())
        {
            var tweet = Tweet.Create(threadId, message, userId, request.AllowedReplies);
            await _repository.AddAsync(tweet, ct);
        }

        var results= await _repository.SaveChangesAsync(ct);
        return results > 0 ? new Result<Unit>(Unit.Value):new Result<Unit>(new Error("Failed to create thread"));
    }
}