using System;
using System.Security.Cryptography;

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
        var provider = new RNGCryptoServiceProvider();
        byte[] data = new byte[size];
        provider.GetBytes(data);
        return Convert.ToBase64String(data);
    }
}