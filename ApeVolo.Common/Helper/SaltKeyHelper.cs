using System;
using System.Security.Cryptography;
using ApeVolo.Common.Extention;

namespace ApeVolo.Common.Helper;

/// <summary>
/// 随机盐
/// </summary>
public static class SaltKeyHelper
{
    /// <summary>
    /// Creates a salt
    /// </summary>
    /// <param name="size">A salt size</param>
    /// <returns>A salt</returns>
    public static string CreateSalt(int size)
    {
        var random = RandomNumberGenerator.Create();
        var bytes = new byte[size];
        random.GetNonZeroBytes(bytes);
        return bytes.ToBase64String();
    }
}