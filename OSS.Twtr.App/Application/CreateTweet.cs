using FluentValidation;
using OSS.Twtr.App.Domain.Repositories;
using OSS.Twtr.App.Domain.ValueObjects;
using OSS.Twtr.Application;
using OSS.Twtr.Core;

namespace OSS.Twtr.App.Application;

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