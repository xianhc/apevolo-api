using ApeVolo.Common.DI;
using ApeVolo.Entity.Do.Base;
using SqlSugar;

namespace ApeVolo.Entity.Do.Logs;

/// <summary>
/// 系统异常日志
/// </summary>
[SugarTable("log_exception", "系统异常日志")]
public class ExceptionLog : EntityRoot<long>, ILocalizedTable
{
    /// <summary>
    /// 区域
    /// </summary>
    [SugarColumn(ColumnName = "area", IsNullable = true, ColumnDescription = "区域")]
    public string Area { get; set; }

    /// <summary>
    /// 控制器
    /// </summary>
    [SugarColumn(ColumnName = "controller", IsNullable = true, ColumnDescription = "控制器")]
    public string Controller { get; set; }

    /// <summary>
    /// 方法
    /// </summary>
    [SugarColumn(ColumnName = "action", IsNullable = true, ColumnDescription = "方法")]
    public string Action { get; set; }

    /// <summary>
    /// 请求方式
    /// </summary>
    [SugarColumn(ColumnName = "method", IsNullable = true, ColumnDescription = "请求方式")]
    public string Method { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    [SugarColumn(ColumnName = "description", IsNullable = true, ColumnDescription = "方法描述")]
    public string Description { get; set; }

    /// <summary>
    /// 请求url
    /// </summary>
    [SugarColumn(ColumnName = "request_url", IsNullable = true, ColumnDescription = "请求URL")]
    public string RequestUrl { get; set; }

    /// <summary>
    /// 请求参数
    /// </summary>
    [SugarColumn(ColumnName = "request_parameters", Length = 5000, IsNullable = true, ColumnDescription = "请求参数")]
    public string RequestParameters { get; set; }

    /// <summary>
    /// 异常短信息
    /// </summary>
    [SugarColumn(ColumnName = "exception_message", IsNullable = true, ColumnDescription = "异常短信息")]
    public string ExceptionMessage { get; set; }

    /// <summary>
    /// 异常完整信息
    /// </summary>
    [SugarColumn(ColumnName = "exception_message_full", Length = 2048, IsNullable = true,
        ColumnDescription = "异常完整信息")]
    public string ExceptionMessageFull { get; set; }

    /// <summary>
    /// 异常堆栈信息
    /// </summary>
    [SugarColumn(ColumnName = "exception_stack", Length = 2048, IsNullable = true, ColumnDescription = "异常堆栈信息")]
    public string ExceptionStack { get; set; }

    /// <summary>
    /// 等级
    /// </summary>
    [SugarColumn(ColumnName = "log_level", IsNullable = true, ColumnDescription = "异常类型")]
    public int LogLevel { get; set; }

    /// <summary>
    /// 请求ip
    /// </summary>
    [SugarColumn(ColumnName = "request_ip", IsNullable = true, ColumnDescription = "请求IP")]
    public string RequestIp { get; set; }

    /// <summary>
    /// ip所属真实地址
    /// </summary>
    [SugarColumn(ColumnName = "ip_address", IsNullable = true, ColumnDescription = "IP所属地址")]
    public string IpAddress { get; set; }

    /// <summary>
    /// 操作系统
    /// </summary>
    [SugarColumn(ColumnName = "operating_system", IsNullable = true, ColumnDescription = "操作系统")]
    public string OperatingSystem { get; set; }

    /// <summary>
    /// 设备类型
    /// </summary>
    [SugarColumn(ColumnName = "device_type", IsNullable = true, ColumnDescription = "设备类型")]
    public string DeviceType { get; set; }

    /// <summary>
    /// 浏览器名称
    /// </summary>
    [SugarColumn(ColumnName = "browser_name", IsNullable = true, ColumnDescription = "浏览器名称")]
    public string BrowserName { get; set; }

    /// <summary>
    /// 浏览器版本
    /// </summary>
    [SugarColumn(ColumnName = "version", IsNullable = true, ColumnDescription = "浏览器版本")]
    public string Version { get; set; }
}