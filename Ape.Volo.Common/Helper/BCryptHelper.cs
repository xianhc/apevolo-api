namespace Ape.Volo.Common.Helper;

/// <summary>
/// 哈希密码、验证
/// </summary>
public static class BCryptHelper
{
    /// <summary>
    /// 密码哈希
    /// </summary>
    /// <param name="inputKey"></param>
    /// <returns></returns>
    public static string Hash(string inputKey)
    {
        var salt = BCrypt.Net.BCrypt.GenerateSalt();
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(inputKey, salt);
        return hashedPassword;
    }

    /// <summary>
    /// 验证密码
    /// </summary>
    /// <param name="inputKey"></param>
    /// <param name="hashKey"></param>
    /// <returns></returns>
    public static bool Verify(string inputKey, string hashKey)
    {
        return BCrypt.Net.BCrypt.Verify(inputKey, hashKey);
    }
}
