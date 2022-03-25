using System.Linq;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ApeVolo.Common.Extention;

/// <summary>
/// 过滤器上下文扩展
/// </summary>
public static partial class ExtObject
{
    /// <summary>
    /// 是否拥有某过滤器
    /// </summary>
    /// <typeparam name="T">过滤器类型</typeparam>
    /// <param name="actionExecutingContext">上下文</param>
    /// <returns></returns>
    public static bool ContainsFilter<T>(this ActionExecutingContext actionExecutingContext)
    {
        return actionExecutingContext.Filters.Any(x => x.GetType() == typeof(T));
    }
}