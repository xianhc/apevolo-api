namespace Ape.Volo.Common.WebApp;

/// <summary>
/// 当前用户
/// </summary>
public interface IHttpUser
{
    /// <summary>
    /// 当前登录用户ID
    /// </summary>
    long Id { get; }

    /// <summary>
    /// 当前登录用户名称
    /// </summary>
    string Account { get; }


    /// <summary>
    /// 请求携带的Token
    /// </summary>
    /// <returns></returns>
    string JwtToken { get; }

    /// <summary>
    /// 是否已认证
    /// </summary>
    /// <returns></returns>
    bool IsAuthenticated { get; }
}
