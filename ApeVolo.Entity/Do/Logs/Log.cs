using ApeVolo.Common.DI;
using ApeVolo.Entity.Do.Base;
using SqlSugar;

namespace ApeVolo.Entity.Do.Logs;

/// <summary>
/// 系统异常日志
/// </summary>
[SugarTable("sys_log", "系统异常日志")]
public class Log : EntityRoot<long>, ILocalizedTable
{
    /// <summary>
    /// 区域
    /// </summary>
    [SugarColumn(ColumnName = "area", ColumnDataType = "varchar", Length = 255, IsNullable = true,
        ColumnDescription = "区域")]
    public string Area { get; set; }

    /// <summary>
    /// 控制器
    /// </summary>
    [SugarColumn(ColumnName = "controller", ColumnDataType = "varchar", Length = 255, IsNullable = true,
        ColumnDescription = "控制器")]
    public string Controller { get; set; }

    /// <summary>
    /// 方法
    /// </summary>
    [SugarColumn(ColumnName = "action", ColumnDataType = "varchar", Length = 255, IsNullable = true,
        ColumnDescription = "方法")]
    public string Action { get; set; }

    /// <summary>
    /// 请求方式
    /// </summary>
    [SugarColumn(ColumnName = "method", ColumnDataType = "varchar", Length = 255, IsNullable = true,
        ColumnDescription = "请求方式")]
    public string Method { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    [SugarColumn(ColumnName = "description", ColumnDataType = "varchar", Length = 255, IsNullable = true,
        ColumnDescription = "方法描述")]
    public string Description { get; set; }

    /// <summary>
    /// 请求url
    /// </summary>
    [SugarColumn(ColumnName = "request_url", ColumnDataType = "varchar", Length = 255, IsNullable = true,
        ColumnDescription = "请求URL")]
    public string RequestUrl { get; set; }

    /// <summary>
    /// 请求参数
    /// </summary>
    [SugarColumn(ColumnName = "request_parameters", ColumnDataType = "longtext", IsNullable = true,
        ColumnDescription = "请求参数")]
    public string RequestParameters { get; set; }

    /// <summary>
    /// 异常短信息
    /// </summary>
    [SugarColumn(ColumnName = "exception_message", ColumnDataType = "longtext", IsNullable = true,
        ColumnDescription = "异常短信息")]
    public string ExceptionMessage { get; set; }

    /// <summary>
    /// 异常完整信息
    /// </summary>
    [SugarColumn(ColumnName = "exception_message_full", ColumnDataType = "longtext", IsNullable = true,
        ColumnDescription = "异常完整信息")]
    public string ExceptionMessageFull { get; set; }

    /// <summary>
    /// 异常堆栈信息
    /// </summary>
    [SugarColumn(ColumnName = "exception_stack", ColumnDataType = "longtext", IsNullable = true,
        ColumnDescription = "异常堆栈信息")]
    public string ExceptionStack { get; set; }

    /// <summary>
    /// 等级
    /// </summary>
    [SugarColumn(ColumnName = "log_level", IsNullable = true, ColumnDescription = "异常类型")]
    public int LogLevel { get; set; }

    /// <summary>
    /// 客户端浏览器信息
    /// </summary>
    [SugarColumn(ColumnName = "browser_info", ColumnDataType = "varchar", Length = 255, IsNullable = true,
        ColumnDescription = "客户端浏览器信息")]
    public string BrowserInfo { get; set; }

    /// <summary>
    /// 请求ip
    /// </summary>
    [SugarColumn(ColumnName = "request_ip", ColumnDataType = "varchar", Length = 255, IsNullable = true,
        ColumnDescription = "请求IP")]
    public string RequestIp { get; set; }

    /// <summary>
    /// ip所属真实地址
    /// </summary>
    [SugarColumn(ColumnName = "ip_address", ColumnDataType = "varchar", Length = 255, IsNullable = true,
        ColumnDescription = "IP所属地址")]
    public string IpAddress { get; set; }
}