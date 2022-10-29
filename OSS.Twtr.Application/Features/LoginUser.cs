using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OSS.Twtr.Application;
using OSS.Twtr.Core;
using OSS.Twtr.Infrastructure;
using OSS.Twtr.Management.Infrastructure.Endpoints;

namespace OSS.Twtr.Identity.Endpoints;

public record struct LoginRequest(string Username, string Password);
public record struct LoginResponse(string Token);
public sealed class LoginEndpoint : TwtrEndpoint<LoginRequest, LoginResponse>
{
    private readonly IMediator _mediator;
    private readonly SignInManager<IdentityUser<Guid>> _signInManager;

    public LoginEndpoint(
        IMediator mediator,
        SignInManager<IdentityUser<Guid>> signInManager)
    {
        _mediator = mediator;
        _signInManager = signInManager;
    }

    public override void Configure()
    {
        Post("/auth/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        var cmdResult = await _mediator.Send(req.Adapt<LoginCommand>(), ct);
        if (!cmdResult.Success)
        {
            await SendResultErrorsAsync(cmdResult.Errors, ct);
            return;
        }
        
        var authenticationProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            AllowRefresh = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(5),
            IssuedUtc = DateTimeOffset.UtcNow,
            Items = {}
        };
            
        cmdResult.Data.Claims.ToList().ForEach(e => authenticationProperties.Items.Add(e.Type, e.Value));

        await _signInManager.SignInAsync(cmdResult.Data.User, authenticationProperties, IdentityConstants.ApplicationScheme);
        await SendAsync(new LoginResponse(cmdResult.Data.BearerToken), cancellation: ct);
    }
}

public record struct LoginCommand(string Username, string Password) : ICommand<Result<AuthenticationResultDto>>;
public record struct AuthenticationResultDto(IdentityUser<Guid> User, IList<Claim> Claims, string BearerToken);
public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Username).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}

internal sealed class LoginHandler : ICommandHandler<LoginCommand, Result<AuthenticationResultDto>>
{
    private readonly UserManager<IdentityUser<Guid>> _userManager;
    private readonly JwtSettings _settings;
    private readonly IEventDispatcher _eventDispatcher;

    public LoginHandler(
        UserManager<IdentityUser<Guid>> userManager,
        IOptions<JwtSettings> settings,
        IEventDispatcher eventDispatcher)
    {
        _userManager = userManager;
        _settings = settings.Value;
        _eventDispatcher = eventDispatcher;
    }

    public async Task<Result<AuthenticationResultDto>> Handle(LoginCommand request, CancellationToken ct)
    {
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user == null)
            return new Result<AuthenticationResultDto>(new Error("User not found"));

        var passwordResult = await _userManager.CheckPasswordAsync(user, request.Password);
        if(!passwordResult)
            return new Result<AuthenticationResultDto>(new Error("Invalid password"));

        _eventDispatcher.Dispatch(new UserConnected(user.Id));
        
        var claims = await _userManager.GetClaimsAsync(user);
        return new Result<AuthenticationResultDto>(new AuthenticationResultDto(user, claims, GenerateToken(user)));
    }

    private string GenerateToken(IdentityUser<Guid> user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString("D")),
            new Claim(ClaimTypes.Name,user.UserName)
        };
        
        var token = new JwtSecurityToken(_settings.Issuer,
            _settings.Audience,
            claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}