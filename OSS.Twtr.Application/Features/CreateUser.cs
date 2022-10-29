using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using OSS.Twtr.Application;
using OSS.Twtr.Core;
using OSS.Twtr.Infrastructure;
using OSS.Twtr.Management.Infrastructure.Endpoints;

namespace OSS.Twtr.Identity.Endpoints;

public record struct CreateUserRequest(string Username, string Password, string ConfirmPassword);
public sealed class CreateUserEndpoint : TwtrEndpoint<CreateUserRequest, Guid>
{
    private readonly IMediator _mediator;
    public CreateUserEndpoint(IMediator mediator) => _mediator = mediator;

    public override void Configure()
    {
        Post("/auth/register");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateUserRequest req, CancellationToken ct)
    {
        var result = await _mediator.Send(req.Adapt<CreateUserCommand>(), ct);
        await result.On(
            createdUserId => SendAsync(createdUserId, cancellation: ct), 
            errors => SendResultErrorsAsync(errors, ct));
    }
}

public record struct CreateUserCommand(string Username, string Password, string ConfirmPassword) : ICommand<Result<Guid>>;
public sealed class CreateUserValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Username).NotEmpty().MinimumLength(4);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        RuleFor(x => x.Password).Equal(x => x.ConfirmPassword);
    }
}

public sealed class CreateUserHandler : ICommandHandler<CreateUserCommand, Result<Guid>>
{
    private readonly UserManager<IdentityUser<Guid>> _userManager;
    private readonly IEventDispatcher _eventDispatcher;

    public CreateUserHandler(
        UserManager<IdentityUser<Guid>> userManager,
        IEventDispatcher eventDispatcher)
    {
        _userManager = userManager;
        _eventDispatcher = eventDispatcher;
    }

    public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken ct)
    {
        var user = new IdentityUser<Guid>(request.Username);
        var creationResult = await _userManager.CreateAsync(user, request.Password);

        if (!creationResult.Succeeded)
            return new Result<Guid>(creationResult.Errors.Select(e => new Error(e.Description, e.Code)));

        _eventDispatcher.Dispatch(new UserCreated(user.Id));
        return new Result<Guid>(user.Id);
    }
}