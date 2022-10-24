using FluentValidation;
using OSS.Twtr.Application;
using OSS.Twtr.Core;
using OSS.Twtr.Identity.Domain;
using OSS.Twtr.Identity.Domain.Services;

namespace OSS.Twtr.Identity.Application.Commands;

public class CreateAccountHandler : ICommandHandler<CreateAccountCommand, Result<Account>>
{
    private readonly IHasher _passwordHasher;
    private readonly IIdentityDbContext _db;

    public CreateAccountHandler(
        IHasher passwordHasher,
        IIdentityDbContext db)
    {
        _passwordHasher = passwordHasher;
        _db = db;
    }

    public async Task<Result<Account>> Handle(CreateAccountCommand request, CancellationToken ct)
    {
        var account = new Account(request.Username, _passwordHasher.HashPassword(request.Username, request.Password));
        _db.Accounts.Add(account);
        
        var result = await _db.SaveChanges(ct);
        return result.On<Result<Account>>(success => new (account), errors => new (errors));
    }
}

public record struct CreateAccountCommand(string Username, string Password, string ConfirmPassword) : ICommand<Result<Account>>;

public sealed class CreateAccountValidator : AbstractValidator<CreateAccountCommand>
{
    public CreateAccountValidator()
    {
        RuleFor(x => x.Username).NotEmpty().MinimumLength(4);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        RuleFor(x => x.Password).Equal(x => x.ConfirmPassword);
    }
}