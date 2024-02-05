using System.Collections.Generic;
using System.Linq;

namespace Ape.Volo.Common.Model;

/// <summary>
/// 错误
/// </summary>
public class ActionError
{
    /// <summary>
    /// 错误集合
    /// </summary>
    public Dictionary<string, string> Errors { get; set; }

    /// <summary>
    /// 第一个错误
    /// </summary>
    /// <returns></returns>
    public string GetFirstError()
    {
        return Errors != null && Errors.Any()
            ? Errors.First().Value
            : "";
    }
}
