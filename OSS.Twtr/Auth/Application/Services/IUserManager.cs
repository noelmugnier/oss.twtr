using System.Security.Claims;
using OSS.Twtr.Auth.Domain.Entities;
using OSS.Twtr.Common.Core;

namespace OSS.Twtr.Auth.Application.Services;

public interface IUserManager
{
    Task<Result<Unit>> CreateAsync(ApplicationUser user, string password, CancellationToken token);
    Task<Result<ApplicationUser>> FindByNameAsync(string username, CancellationToken token);
    Task<Result<bool>> CheckPasswordAsync(ApplicationUser user, string password, CancellationToken token);
    Task<Result<IList<Claim>>> GetClaimsAsync(ApplicationUser user, CancellationToken token);
}