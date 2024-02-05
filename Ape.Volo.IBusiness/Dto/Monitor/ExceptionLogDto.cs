using Ape.Volo.Common.AttributeExt;
using Ape.Volo.Entity.Monitor;
using Ape.Volo.IBusiness.Base;

namespace Ape.Volo.IBusiness.Dto.Monitor;

/// <summary>
/// 系统异常Dto
/// </summary>
[AutoMapping(typeof(ExceptionLog), typeof(ExceptionLogDto))]
public class ExceptionLogDto : BaseEntityDto<long>
{
    /// <summary>
    /// 区
    /// </summary>
    public string Area { get; set; }

    /// <summary>
    /// 控制器
    /// </summary>
    public string Controller { get; set; }

    /// <summary>
    /// 方法
    /// </summary>
    public string Action { get; set; }

    /// <summary>
    /// 请求方法
    /// </summary>
    public string Method { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 请求Url
    /// </summary>
    public string RequestUrl { get; set; }

    /// <summary>
    /// 请求参数
    /// </summary>
    public string RequestParameters { get; set; }

    /// <summary>
    /// 异常消息
    /// </summary>
    public string ExceptionMessage { get; set; }

    /// <summary>
    /// 异常完整消息
    /// </summary>
    public string ExceptionMessageFull { get; set; }

    /// <summary>
    /// 异常堆栈信息
    /// </summary>
    public string ExceptionStack { get; set; }

    /// <summary>
    /// 日志等级
    /// </summary>
    public int LogLevel { get; set; }

    /// <summary>
    /// 请求IP
    /// </summary>
    public string RequestIp { get; set; }

    /// <summary>
    /// IP地址
    /// </summary>
    public string IpAddress { get; set; }

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
    /// 版本号
    /// </summary>
    public string Version { get; set; }
}
