using System;
using Ape.Volo.Common.Extention;

namespace Ape.Volo.Common.Helper;

/// <summary>
/// GUID帮助类
/// </summary>
public static class GuidHelper
{
    /// <summary>
    /// 生成主键
    /// </summary>
    /// <returns></returns>
    public static string GenerateKey()
    {
        return Guid.NewGuid().ToSequentialGuid().ToUpper();
    }
}
