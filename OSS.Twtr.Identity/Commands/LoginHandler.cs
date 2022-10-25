using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OSS.Twtr.Application;
using OSS.Twtr.Core;
using OSS.Twtr.Identity.Contracts;

namespace OSS.Twtr.Identity.Application.Commands;

public class LoginHandler : ICommandHandler<LoginCommand, Result<AuthenticationResultDto>>
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly JwtSettings _settings;
    private readonly IEventDispatcher _eventDispatcher;

    public LoginHandler(
        UserManager<IdentityUser> userManager,
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

        _eventDispatcher.Dispatch(new UserConnected((UserId)user.Id));
        
        var claims = await _userManager.GetClaimsAsync(user);
        return new Result<AuthenticationResultDto>(new AuthenticationResultDto(user, claims, GenerateToken(user)));
    }

    private string GenerateToken(IdentityUser user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier,user.Id),
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

public record struct LoginCommand(string Username, string Password) : ICommand<Result<AuthenticationResultDto>>;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Username).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}