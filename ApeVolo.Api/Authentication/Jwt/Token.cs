using Newtonsoft.Json;

namespace ApeVolo.Api.Authentication.Jwt;

public class Token
{
    /// <summary>
    /// 授权token
    /// </summary>
    [JsonProperty("access_token")]
    public string AccessToken { get; set; }

    /// <summary>
    /// 过期时间
    /// </summary>
    [JsonProperty("expires_in")]
    public int Expires { get; set; }

    /// <summary>
    /// 类型
    /// </summary>
    [JsonProperty("token_type")]
    public string TokenType { get; set; }

    /// <summary>
    /// 刷新token
    /// </summary>
    [JsonProperty("refresh_token")]
    public string RefreshToken { get; set; }

    /// <summary>
    /// 允许token时间内
    /// </summary>
    [JsonProperty("refresh_token_expires_in")]
    public int RefreshTokenExpires { get; set; }
}
