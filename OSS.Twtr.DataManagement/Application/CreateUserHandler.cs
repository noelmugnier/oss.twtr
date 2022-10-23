using FluentValidation;
using OSS.Twtr.Application;
using OSS.Twtr.Core;
using OSS.Twtr.Domain.Ids;
using OSS.Twtr.TweetManagement.Domain;
using OSS.Twtr.TweetManagement.Infrastructure;

namespace OSS.Twtr.TweetManagement.Application;

public class CreateUserHandler : ICommandHandler<CreateUserCommand, Result<User>>
{
    private readonly IDataDbContext _db;

    public CreateUserHandler(IDataDbContext db)
    {
        _db = db;
    }

    public async Task<Result<User>> Handle(CreateUserCommand request, CancellationToken ct)
    {
        var user = new User(UserId.From(request.UserId), request.Username, request.CreatedOn);
        _db.Users.Add(user);
        var result = await _db.SaveChanges(ct);
        
        return result.On<Result<User>>(success => new(user), errors => new(errors));
    }
}

public record struct CreateUserCommand(Guid UserId, string Username, DateTimeOffset CreatedOn): ICommand<Result<User>>;

public sealed class CreateUserValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.UserId).NotEqual(Guid.Empty);
        RuleFor(x => x.Username).NotEmpty().MinimumLength(1);
    }
}