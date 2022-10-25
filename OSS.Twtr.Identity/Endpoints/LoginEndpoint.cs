using FastEndpoints;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using OSS.Twtr.Identity.Application.Commands;

namespace OSS.Twtr.Identity.Endpoints;

public class LoginEndpoint : Endpoint<LoginRequest, LoginResponse>
{
    private readonly IMediator _mediator;
    private readonly SignInManager<IdentityUser> _signInManager;

    public LoginEndpoint(
        IMediator mediator,
        SignInManager<IdentityUser> signInManager)
    {
        _mediator = mediator;
        _signInManager = signInManager;
    }

    public override void Configure()
    {
        Post("/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        var result = await _mediator.Send(req.Adapt<LoginCommand>(), ct);
        
        await result.On(async success =>
        {
            var authenticationProperties = new AuthenticationProperties
            {
                IsPersistent = req.RememberLogin,
                AllowRefresh = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(5),
                IssuedUtc = DateTimeOffset.UtcNow,
                Items = {}
            };
            
            success.Claims.ToList().ForEach(e => authenticationProperties.Items.Add(e.Type, e.Value));

            await _signInManager.SignInAsync(success.User, authenticationProperties, IdentityConstants.ApplicationScheme);
            await SendAsync(new LoginResponse(success.BearerToken), cancellation: ct);
        }, errors =>
        {
            foreach (var error in errors)
                AddError(error.Message, error.ErrorCode, (Severity)error.Severity);

            return SendErrorsAsync(cancellation: ct);
        });
    }
}

public record struct LoginRequest(string Username, string Password, bool RememberLogin);
public record struct LoginResponse(string Token);