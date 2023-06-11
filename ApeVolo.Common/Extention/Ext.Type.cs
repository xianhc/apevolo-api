using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ApeVolo.Common.Extention;

public static partial class ExtObject
{
    public static Dictionary<string, List<PropertyInfo>> _propertyCache { get; set; } =
        new Dictionary<string, List<PropertyInfo>>();

    /// <summary>
    /// 判断是否是泛型
    /// </summary>
    /// <param name="self">Type类</param>
    /// <param name="innerType">泛型类型</param>
    /// <returns>判断结果</returns>
    public static bool IsGeneric(this Type self, Type innerType)
    {
        if (self.GetTypeInfo().IsGenericType && self.GetGenericTypeDefinition() == innerType)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 判断是否为Nullable<>类型
    /// </summary>
    /// <param name="self">Type类</param>
    /// <returns>判断结果</returns>
    public static bool IsNullable(this Type self)
    {
        return self.IsGeneric(typeof(Nullable<>));
    }

    /// <summary>
    /// 判断是否为List<>类型
    /// </summary>
    /// <param name="self">Type类</param>
    /// <returns>判断结果</returns>
    public static bool IsList(this Type self)
    {
        return self.IsGeneric(typeof(List<>));
    }

    #region 判断是否为枚举

    /// <summary>
    /// 判断是否为枚举
    /// </summary>
    /// <param name="self">Type类</param>
    /// <returns>判断结果</returns>
    public static bool IsEnum(this Type self)
    {
        return self.GetTypeInfo().IsEnum;
    }

    /// <summary>
    /// 判断是否为枚举或者可空枚举
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    public static bool IsEnumOrNullableEnum(this Type self)
    {
        if (self == null)
        {
            return false;
        }

        if (self.IsEnum)
        {
            return true;
        }
        else
        {
            if (self.IsGenericType && self.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                self.GetGenericArguments()[0].IsEnum)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    #endregion

    /// <summary>
    /// 判断是否为值类型
    /// </summary>
    /// <param name="self">Type类</param>
    /// <returns>判断结果</returns>
    public static bool IsPrimitive(this Type self)
    {
        return self.GetTypeInfo().IsPrimitive || self == typeof(decimal);
    }

    public static bool IsNumber(this Type self)
    {
        Type checktype = self;
        if (self.IsNullable())
        {
            checktype = self.GetGenericArguments()[0];
        }

        if (checktype == typeof(int) || checktype == typeof(short) || checktype == typeof(long) ||
            checktype == typeof(float) || checktype == typeof(decimal) || checktype == typeof(double))
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    #region 判断是否是Bool

    public static bool IsBool(this Type self)
    {
        return self == typeof(bool);
    }

    /// <summary>
    /// 判断是否是 bool or bool?类型
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    public static bool IsBoolOrNullableBool(this Type self)
    {
        if (self == null)
        {
            return false;
        }

        if (self == typeof(bool) || self == typeof(bool?))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion

    public static PropertyInfo GetSingleProperty(this Type self, string name)
    {
        if (_propertyCache.ContainsKey(self.FullName) == false)
        {
            _propertyCache.Add(self.FullName, self.GetProperties().ToList());
        }

        return _propertyCache[self.FullName].Where(x => x.Name == name).FirstOrDefault();
    }
}