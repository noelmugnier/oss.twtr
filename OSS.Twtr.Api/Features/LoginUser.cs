using Mapster;
using OSS.Twtr.Application;
using OSS.Twtr.Auth.Application;
using OSS.Twtr.Auth.Application.Services;
using OSS.Twtr.Common.Infrastructure;
using OSS.Twtr.Core;

namespace OSS.Twtr.Api.Features;

public record LoginUserRequest
{
    public string Username { get; init; }
    public string Password { get; init; }
    public bool RememberMe { get; init; } = false;
}

public record struct LoginUserResponse(string Token);

public sealed class LoginUserEndpoint : TwtrEndpoint<LoginUserRequest, LoginUserResponse>
{
    private readonly ICommandDispatcher _mediator;
    private readonly ISignInManager _signInManager;

    public LoginUserEndpoint(
        ICommandDispatcher mediator,
        ISignInManager signInManager)
    {
        _mediator = mediator;
        _signInManager = signInManager;
    }

    public override void Configure()
    {
        Post("/auth/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginUserRequest req, CancellationToken ct)
    {
        var cmdResult = await _mediator.Execute(req.Adapt<LoginUserCommand>(), ct);
        if (!cmdResult.Success)
        {
            await SendResultErrorsAsync(cmdResult.Errors, ct);
            return;
        }

        try
        {
            var loginResult = cmdResult.Data;
            await _signInManager.SignInAsync(loginResult, req.RememberMe, ct);
            await SendAsync(new LoginUserResponse(loginResult.BearerToken), cancellation: ct);
        }
        catch (Exception e)
        {
            await SendResultErrorsAsync(new List<Error>{new Error(e.Message)}, ct);
        }
    }
}