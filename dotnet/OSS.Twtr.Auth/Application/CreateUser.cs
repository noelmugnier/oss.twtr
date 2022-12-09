using FluentValidation;
using OSS.Twtr.Application;
using OSS.Twtr.Auth.Application.Services;
using OSS.Twtr.Auth.Domain.Entities;
using OSS.Twtr.Auth.Domain.Events;
using OSS.Twtr.Core;
using OSS.Twtr.Domain;

namespace OSS.Twtr.Auth.Application;

public record struct CreateUserCommand(string Username, string Password, string ConfirmPassword) : ICommand<Result<Guid>>;
public sealed class CreateUserValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Username).NotEmpty().MinimumLength(4);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        RuleFor(x => x.ConfirmPassword).Equal(x => x.Password);
    }
}

internal class CreateUserHandler : ICommandHandler<CreateUserCommand, Result<Guid>>
{
    private readonly IUserManager  _userManager;
    private readonly IEventDispatcher _eventDispatcher;

    public CreateUserHandler(
        IUserManager userManager,
        IEventDispatcher eventDispatcher)
    {
        _userManager = userManager;
        _eventDispatcher = eventDispatcher;
    }

    public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken ct)
    {
        if (request.Password != request.ConfirmPassword)
            return new Result<Guid>(new Error("Password and Confirmation do not match"));

        try
        {
            var user = new ApplicationUser(request.Username);
            var creationResult = await _userManager.CreateAsync(user, request.Password, ct);
            if (!creationResult.Success)
                return new Result<Guid>(creationResult.Errors);

            _eventDispatcher.Dispatch(new UserCreated(user.Id));
            return new Result<Guid>(user.Id);
        }
        catch (Exception e)
        {
            return new Result<Guid>(e);
        }
    }
}