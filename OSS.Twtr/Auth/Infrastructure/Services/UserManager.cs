using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OSS.Twtr.Auth.Application.Services;
using OSS.Twtr.Auth.Domain.Entities;
using OSS.Twtr.Common.Core;

namespace OSS.Twtr.Auth.Infrastructure.Services;

public class ApplicationUserManager : UserManager<ApplicationUser>, IUserManager
{
    public ApplicationUserManager(
        IUserStore<ApplicationUser> store, 
        IOptions<IdentityOptions> optionsAccessor, 
        IPasswordHasher<ApplicationUser> passwordHasher, 
        IEnumerable<IUserValidator<ApplicationUser>> userValidators, 
        IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators, 
        ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, 
        IServiceProvider services, 
        ILogger<ApplicationUserManager> logger) 
        : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
    {
    }

    public async Task<Result<Unit>> CreateAsync(ApplicationUser user, string password, CancellationToken token)
    {
        try
        {
            var result = await base.CreateAsync(user, password);
            return result.Succeeded 
                ? new Result<Unit>(Unit.Value) 
                : new Result<Unit>(result.Errors.Select(e => new Error($"{e.Code}: {e.Description}")));
        }
        catch (Exception e)
        {
            return new Result<Unit>(e);
        }
    }

    public async Task<Result<ApplicationUser>> FindByNameAsync(string username, CancellationToken token)
    {
        try
        {
            var result = await base.FindByNameAsync(username);
            return result != null 
                ? new Result<ApplicationUser>(result) 
                : new Result<ApplicationUser>(new Error("Invalid username or password"));
        }
        catch (Exception e)
        {
            return new Result<ApplicationUser>(e);
        }
    }

    public async Task<Result<bool>> CheckPasswordAsync(ApplicationUser user, string password, CancellationToken token)
    {
        try
        {
            var result = await base.CheckPasswordAsync(user, password);
            return result 
                ? new Result<bool>(result)
                : new Result<bool>(new Error("Invalid username or password"));
        }
        catch (Exception e)
        {
            return new Result<bool>(e);
        }
    }

    public async Task<Result<IList<Claim>>> GetClaimsAsync(ApplicationUser user, CancellationToken token)
    {
        try
        {
            var result = await base.GetClaimsAsync(user);
            return new Result<IList<Claim>>(result);
        }
        catch (Exception e)
        {
            return new Result<IList<Claim>>(e);
        }
    }
}