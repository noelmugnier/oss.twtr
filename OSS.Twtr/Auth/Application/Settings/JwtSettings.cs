namespace OSS.Twtr.Auth.Application.Settings;

public class JwtSettings
{
    public const string Section = "Jwt";
    public string Key { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
}