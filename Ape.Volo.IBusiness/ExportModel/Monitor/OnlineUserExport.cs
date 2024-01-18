using System.ComponentModel.DataAnnotations;
using Ape.Volo.Common.Model;

namespace Ape.Volo.IBusiness.ExportModel.Monitor;

public class OnlineUserExport : ExportBase
{
    [Display(Name = "用户名称")]
    public string Account { get; set; }

    [Display(Name = "用户昵称")]
    public string NickName { get; set; }

    [Display(Name = "部门名称")]
    public string Dept { get; set; }

    [Display(Name = "登录IP")]
    public string Ip { get; set; }

    [Display(Name = "IP地址")]
    public string Address { get; set; }

    [Display(Name = "操作系统")]
    public string OperatingSystem { get; set; }

    [Display(Name = "设备类型")]
    public string DeviceType { get; set; }

    [Display(Name = "浏览器名称")]
    public string BrowserName { get; set; }

    [Display(Name = "版本号")]
    public string Version { get; set; }

    [Display(Name = "登录时间")]
    public string LoginTime { get; set; }

    [Display(Name = "令牌")]
    public string AccessToken { get; set; }
}
