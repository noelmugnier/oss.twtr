using Microsoft.AspNetCore.Identity;
using OSS.Twtr.Domain.Services;

namespace OSS.Twtr.Infrastructure.Services;

public class Hasher : IHasher
{
    private readonly IPasswordHasher<object> _passwordHasher;

    public Hasher(IPasswordHasher<object> passwordHasher)
    {
        _passwordHasher = passwordHasher;
    }

    public string HashPassword(string username, string password)
    {
        return _passwordHasher.HashPassword(username, password);
    }

    public bool VerifyHashedPassword(string username, string hashedPassword, string providedPassword)
    {
        var result = _passwordHasher.VerifyHashedPassword(username, hashedPassword, providedPassword);
        return result == PasswordVerificationResult.Success;
    }
}