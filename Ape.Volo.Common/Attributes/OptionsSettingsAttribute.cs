using System;

namespace Ape.Volo.Common.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class OptionsSettingsAttribute : Attribute
{
    /// <summary>
    /// 构造函数
    /// </summary>
    public OptionsSettingsAttribute()
    {
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="path">appsetting.json 对应键</param>
    public OptionsSettingsAttribute(string path)
    {
        Path = path;
    }

    /// <summary>
    /// 对应配置文件中的路径
    /// </summary>
    public string Path { get; set; }
}
