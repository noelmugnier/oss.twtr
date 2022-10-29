namespace OSS.Twtr.Identity.Endpoints;

public class JwtSettings
{
    public const string Section = "Jwt";
    public string Key { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
}