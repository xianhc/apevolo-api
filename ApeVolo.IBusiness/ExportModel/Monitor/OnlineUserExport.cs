using System.ComponentModel.DataAnnotations;
using ApeVolo.Common.Model;

namespace ApeVolo.IBusiness.ExportModel.Monitor;

public class OnlineUserExport : ExportBase
{
    [Display(Name = "User.Name")]
    public string UserName { get; set; }

    [Display(Name = "User.NickName")]
    public string NickName { get; set; }

    [Display(Name = "Dept.Name")]
    public string Dept { get; set; }

    [Display(Name = "Online.LoginIp")]
    public string Ip { get; set; }

    [Display(Name = "Online.IpAddress")]
    public string Address { get; set; }

    [Display(Name = "Online.OperatingSystem")]
    public string OperatingSystem { get; set; }

    [Display(Name = "Online.DeviceType")]
    public string DeviceType { get; set; }

    [Display(Name = "Online.BrowserName")]
    public string BrowserName { get; set; }

    [Display(Name = "Online.Version")]
    public string Version { get; set; }

    [Display(Name = "Online.LoginTime")]
    public string LoginTime { get; set; }

    [Display(Name = "Online.Onlinekey")]
    public string OnlineKey { get; set; }
}