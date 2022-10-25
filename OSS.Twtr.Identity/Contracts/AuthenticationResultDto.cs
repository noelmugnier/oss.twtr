using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace OSS.Twtr.Identity.Application.Commands;

public record struct AuthenticationResultDto(IdentityUser User, IList<Claim> Claims, string BearerToken);