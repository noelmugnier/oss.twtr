using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OSS.Twtr.Auth.Application.Services;
using OSS.Twtr.Auth.Application.Settings;
using OSS.Twtr.Auth.Domain.Entities;
using OSS.Twtr.Auth.Domain.Events;
using OSS.Twtr.Common.Application;
using OSS.Twtr.Common.Core;
using OSS.Twtr.Common.Domain;

namespace OSS.Twtr.Auth.Application;

public record struct LoginUserCommand(string Username, string Password) : ICommand<Result<LoginUserResult>>;

public sealed class LoginUserValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserValidator()
    {
        RuleFor(x => x.Username).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}

public record struct LoginUserResult(ApplicationUser User, IList<Claim> Claims, string BearerToken);

internal sealed class LoginUserHandler : ICommandHandler<LoginUserCommand, Result<LoginUserResult>>
{
    private readonly IUserManager _userManager;
    private readonly JwtSettings _settings;
    private readonly IEventDispatcher _eventDispatcher;

    public LoginUserHandler(
        IUserManager userManager,
        IOptions<JwtSettings> settings,
        IEventDispatcher eventDispatcher)
    {
        _userManager = userManager;
        _settings = settings.Value;
        _eventDispatcher = eventDispatcher;
    }

    public async Task<Result<LoginUserResult>> Handle(LoginUserCommand request, CancellationToken ct)
    {
        try
        {
            var userResult = await _userManager.FindByNameAsync(request.Username, ct);
            if (!userResult.Success)
                return new Result<LoginUserResult>(userResult.Errors);

            var user = userResult.Data;
            var passwordResult = await _userManager.CheckPasswordAsync(user, request.Password, ct);
            if (!passwordResult.Success)
                return new Result<LoginUserResult>(passwordResult.Errors);

            var claimsResult = await _userManager.GetClaimsAsync(user, ct);
            if (!claimsResult.Success)
                return new Result<LoginUserResult>(claimsResult.Errors);
            
            var bearerToken = GenerateToken(user);
            
            _eventDispatcher.Dispatch(new UserConnected(user.Id));
            return new Result<LoginUserResult>(new LoginUserResult(user, claimsResult.Data, bearerToken));
        }
        catch (Exception e)
        {
            return new Result<LoginUserResult>(e);
        }
    }

    private string GenerateToken(IdentityUser<Guid> user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString("D")),
            new Claim(ClaimTypes.Name, user.UserName)
        };

        var token = new JwtSecurityToken(_settings.Issuer,
            _settings.Audience,
            claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}