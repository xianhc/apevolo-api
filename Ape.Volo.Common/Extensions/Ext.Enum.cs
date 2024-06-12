using System;
using System.ComponentModel.DataAnnotations;

namespace Ape.Volo.Common.Extensions;

public static partial class ExtObject
{
    /// <summary>
    /// 获取枚举显示名称
    /// </summary>
    /// <param name="enumValue"></param>
    /// <returns></returns>
    public static string GetDisplayName(this Enum enumValue)
    {
        // 获取枚举类型
        var type = enumValue.GetType();
        // 获取枚举成员的成员信息
        var memberInfo = type.GetMember(enumValue.ToString());

        if (memberInfo.Length > 0)
        {
            var attributes = memberInfo[0].GetCustomAttributes(typeof(DisplayAttribute), false);
            if (attributes.Length > 0)
            {
                return ((DisplayAttribute)attributes[0]).Name;
            }
        }

        return enumValue.ToString();
    }
}
