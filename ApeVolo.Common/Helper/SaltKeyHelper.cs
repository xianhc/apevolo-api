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
#pragma warning disable CS0618
        var provider = new RNGCryptoServiceProvider();
#pragma warning restore CS0618
        byte[] data = new byte[size];
        provider.GetBytes(data);
        return Convert.ToBase64String(data);
    }
}