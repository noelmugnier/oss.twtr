using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OSS.Twtr.Auth.Application;
using OSS.Twtr.Auth.Application.Services;
using OSS.Twtr.Auth.Domain.Entities;
using OSS.Twtr.Core;

namespace OSS.Twtr.Auth.Infrastructure.Services;

public class ApplicationSignInManager : SignInManager<ApplicationUser>, ISignInManager
{
    public ApplicationSignInManager(
        UserManager<ApplicationUser> userManager, 
        IHttpContextAccessor contextAccessor, 
        IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory, 
        IOptions<IdentityOptions> optionsAccessor, 
        ILogger<ApplicationSignInManager> logger, 
        IAuthenticationSchemeProvider schemes, 
        IUserConfirmation<ApplicationUser> confirmation)
        : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
    {
    }

    public async Task<Result<Unit>> SignInAsync(LoginUserResult loginResult, bool rememberMe, CancellationToken token)
    {
        try
        {
            var properties = new AuthenticationProperties
            {
                IsPersistent = rememberMe,
                AllowRefresh = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30),
                IssuedUtc = DateTimeOffset.UtcNow,
                Items = { }
            };
            
            loginResult.Claims.ToList().ForEach(e => properties.Items.Add(e.Type, e.Value));
            await base.SignInAsync(loginResult.User, properties, IdentityConstants.ApplicationScheme);
            return new Result<Unit>(Unit.Value);
        }
        catch (Exception e)
        {
            return new Result<Unit>(e);
        }
    }
}