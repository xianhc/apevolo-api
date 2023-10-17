using System;

namespace ApeVolo.Common.WebApp;

/// <summary>
/// 在线用户
/// </summary>
public class LoginUserInfo
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    public string Account { get; set; }

    /// <summary>
    /// 用户昵称
    /// </summary>
    public string NickName { get; set; }

    /// <summary>
    /// 部门ID
    /// </summary>
    public long DeptId { get; set; }

    /// <summary>
    /// 用户部门
    /// </summary>
    public string DeptName { get; set; }


    /// <summary>
    /// 请求IP
    /// </summary>
    public string Ip { get; set; }

    /// <summary>
    /// IP详细地址
    /// </summary>
    public string Address { get; set; }

    /// <summary>
    /// 操作系统
    /// </summary>
    public string OperatingSystem { get; set; }

    /// <summary>
    /// 设备类型
    /// </summary>
    public string DeviceType { get; set; }

    /// <summary>
    /// 浏览器名称
    /// </summary>
    public string BrowserName { get; set; }

    /// <summary>
    /// 浏览器版本
    /// </summary>
    public string Version { get; set; }

    /// <summary>
    /// 在线唯一表示KEY
    /// </summary>
    public string AccessToken { get; set; }

    /// <summary>
    /// 登录时间
    /// </summary>
    public DateTime LoginTime { get; set; }

    /// <summary>
    /// 当前权限信息
    /// </summary>
    public CurrentPermission CurrentPermission { get; set; }
}
