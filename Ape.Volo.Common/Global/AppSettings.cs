using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace Ape.Volo.Common.Global;

/// <summary>
/// 配置文件帮助类
/// </summary>
public class AppSettings
{
    public static IConfiguration Configuration { get; set; }

    public AppSettings(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    #region 使用Configs

    /// <summary>
    /// 从AppSettings获取key的值
    /// </summary>
    /// <param name="key">key</param>
    /// <returns></returns>
    public static string GetValue(string key)
    {
        try
        {
            return Configuration[key];
        }
        catch
        {
            // ignored
        }

        return "";
    }

    /// <summary>
    /// 从AppSettings获取key的值
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="key">键路径</param>
    /// <returns></returns>
    public static T GetValue<T>(string key)
    {
        return ConvertValue<T>(GetValue(key));
    }


    /// <summary>
    /// 从AppSettings获取key的值
    /// </summary>
    /// <param name="keys"></param>
    /// <returns></returns>
    public static string GetValue(params string[] keys)
    {
        try
        {
            if (keys.Any())
            {
                return Configuration[string.Join(":", keys)];
            }
        }
        catch
        {
            // ignored
        }

        return "";
    }

    /// <summary>
    /// 从AppSettings获取key的值
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="keys">键路径</param>
    /// <returns></returns>
    public static T GetValue<T>(params string[] keys)
    {
        return ConvertValue<T>(GetValue(keys));
    }


    /// <summary>
    /// 值类型转换
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="value">值</param>
    /// <returns></returns>
    public static T ConvertValue<T>(string value)
    {
        return (T)ConvertValue(typeof(T), value);
    }

    /// <summary>
    /// 值类型转换
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="value">值</param>
    /// <returns></returns>
    public static object ConvertValue(Type type, string value)
    {
        if (type == typeof(object))
        {
            return value;
        }

        if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            return string.IsNullOrEmpty(value) ? value : ConvertValue(Nullable.GetUnderlyingType(type), value);
        }

        var converter = TypeDescriptor.GetConverter(type);
        return converter.CanConvertFrom(typeof(string)) ? converter.ConvertFromInvariantString(value) : null;
    }

    #endregion
}
