namespace ApeVolo.Api.Authentication;

/// <summary>
/// 权限列表
/// </summary>
public sealed class PermissionList
{
    public PermissionList(string role, string url)
    {
        Role = role;
        Url = url;
    }

    /// <summary>
    /// 用户或角色或其他凭据名称
    /// </summary>
    public string Role { get; set; }

    /// <summary>
    /// 请求Url
    /// </summary>
    public string Url { get; set; }
}