namespace ApeVolo.Common.ConfigOptions;

public class JwtAuthOption
{
    public string Audience { get; set; }
    public string Issuer { get; set; }
    public string SecurityKey { get; set; }
    public int Expiration { get; set; }
    public int RefreshTokenExpires { get; set; }
    public string LoginPath { get; set; }
}
