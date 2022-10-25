using FluentValidation;
using Microsoft.AspNetCore.Identity;
using OSS.Twtr.Application;
using OSS.Twtr.Core;
using OSS.Twtr.Identity.Contracts;

namespace OSS.Twtr.Identity.Application.Commands;

public class CreateAccountHandler : ICommandHandler<CreateAccountCommand, Result<AccountDto>>
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IEventDispatcher _eventDispatcher;

    public CreateAccountHandler(
        UserManager<IdentityUser> userManager,
        IEventDispatcher eventDispatcher)
    {
        _userManager = userManager;
        _eventDispatcher = eventDispatcher;
    }

    public async Task<Result<AccountDto>> Handle(CreateAccountCommand request, CancellationToken ct)
    {
        var user = new IdentityUser(request.Username);
        var creationResult = await _userManager.CreateAsync(user, request.Password);

        if (!creationResult.Succeeded)
            return new Result<AccountDto>(creationResult.Errors.Select(e => new Error(e.Description, e.Code)));

        _eventDispatcher.Dispatch(new AccountCreated((UserId)user.Id, user.UserName));
        return new Result<AccountDto>(new AccountDto(Guid.Parse(user.Id), user.UserName));
    }
}

public record struct CreateAccountCommand(string Username, string Password, string ConfirmPassword) : ICommand<Result<AccountDto>>;

public sealed class CreateAccountValidator : AbstractValidator<CreateAccountCommand>
{
    public CreateAccountValidator()
    {
        RuleFor(x => x.Username).NotEmpty().MinimumLength(4);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        RuleFor(x => x.Password).Equal(x => x.ConfirmPassword);
    }
}