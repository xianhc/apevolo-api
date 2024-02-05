using System.ComponentModel.DataAnnotations;
using Ape.Volo.Common.Model;

namespace Ape.Volo.IBusiness.ExportModel.Monitor;

/// <summary>
/// 在线用户导出模板
/// </summary>
public class OnlineUserExport : ExportBase
{
    /// <summary>
    /// 用户名称
    /// </summary>
    [Display(Name = "用户名称")]
    public string Account { get; set; }

    /// <summary>
    /// 用户昵称
    /// </summary>
    [Display(Name = "用户昵称")]
    public string NickName { get; set; }

    /// <summary>
    /// 部门名称
    /// </summary>
    [Display(Name = "部门名称")]
    public string Dept { get; set; }

    /// <summary>
    /// 登录IP
    /// </summary>
    [Display(Name = "登录IP")]
    public string Ip { get; set; }

    /// <summary>
    /// IP地址
    /// </summary>
    [Display(Name = "IP地址")]
    public string Address { get; set; }

    /// <summary>
    /// 操作系统
    /// </summary>
    [Display(Name = "操作系统")]
    public string OperatingSystem { get; set; }

    /// <summary>
    /// 设备类型
    /// </summary>
    [Display(Name = "设备类型")]
    public string DeviceType { get; set; }

    /// <summary>
    /// 浏览器名称
    /// </summary>
    [Display(Name = "浏览器名称")]
    public string BrowserName { get; set; }

    /// <summary>
    /// 版本号
    /// </summary>
    [Display(Name = "版本号")]
    public string Version { get; set; }

    /// <summary>
    /// 登录时间
    /// </summary>
    [Display(Name = "登录时间")]
    public string LoginTime { get; set; }

    /// <summary>
    /// 令牌
    /// </summary>
    [Display(Name = "令牌")]
    public string AccessToken { get; set; }
}
