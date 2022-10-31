using Microsoft.AspNetCore.Identity;

namespace OSS.Twtr.Auth.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public ApplicationUser() { }
    public ApplicationUser(string userName):base(userName)
    {
    }
}