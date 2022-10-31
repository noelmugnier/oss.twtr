using FluentValidation;
using OSS.Twtr.Common.Application;
using OSS.Twtr.Common.Core;
using OSS.Twtr.Tweet.Domain.Repositories;
using OSS.Twtr.Tweet.Domain.ValueObjects;

namespace OSS.Twtr.Tweet.Application;

public record struct CreateTweetCommand(Guid UserId, string Message) : ICommand<Result<Guid>>;

internal sealed class CreateTweetValidator : AbstractValidator<CreateTweetCommand>
{
    public CreateTweetValidator()
    {
        RuleFor(x => x.Message).NotEmpty();
        RuleFor(x => x.UserId).NotEqual(Guid.Empty)
            .WithMessage("You must be authenticated to perform this action");
    }
}

internal sealed class CreateTweetHandler : ICommandHandler<CreateTweetCommand, Result<Guid>>
{
    private readonly ITweetRepository _repository;
    public CreateTweetHandler(ITweetRepository repository) => _repository = repository;

    public async Task<Result<Guid>> Handle(CreateTweetCommand request, CancellationToken ct)
    {
        var userResult = await _repository.Get(UserId.From(request.UserId), ct);
        if (!userResult.Success)
            return new Result<Guid>(userResult.Errors);

        var user = userResult.Data;
        var postedTweetId = user.Post(request.Message);

        var result = await _repository.Save(user, ct);
        return result.On(
            success => new Result<Guid>(postedTweetId.Value),
            errors => new Result<Guid>(errors));
    }
}