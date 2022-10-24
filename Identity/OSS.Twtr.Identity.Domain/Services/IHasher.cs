namespace OSS.Twtr.Identity.Domain.Services;

public interface IHasher
{
    string HashPassword(string username, string password);
    bool VerifyHashedPassword(string username, string hashedPassword, string providedPassword);
}