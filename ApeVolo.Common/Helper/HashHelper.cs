namespace ApeVolo.Common.Helper;

using BCrypt.Net;

/// <summary>
/// 哈希密码、验证
/// </summary>
public static class HashHelper
{
    /// <summary>
    /// 生成随机盐
    /// </summary>
    /// <returns></returns>
    public static string GenerateSalt()
    {
        return BCrypt.GenerateSalt();
    }

    /// <summary>
    /// 密码哈希
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    public static string HashPassword(string password)
    {
        var salt = BCrypt.GenerateSalt();
        var hashedPassword = BCrypt.HashPassword(password, salt);
        return hashedPassword;
    }

    /// <summary>
    /// 验证密码
    /// </summary>
    /// <param name="password"></param>
    /// <param name="hashedPassword"></param>
    /// <returns></returns>
    public static bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Verify(password, hashedPassword);
    }
}
