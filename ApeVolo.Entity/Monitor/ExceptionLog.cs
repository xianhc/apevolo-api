using ApeVolo.Common.Model;
using ApeVolo.Entity.Base;
using SqlSugar;

namespace ApeVolo.Entity.Monitor;

/// <summary>
/// 系统异常日志
/// </summary>
[SugarTable("log_exception", "系统异常日志")]
public class ExceptionLog : BaseEntity, ISoftDeletedEntity
{
    /// <summary>
    /// 区域
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "区域")]
    public string Area { get; set; }

    /// <summary>
    /// 控制器
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "控制器")]
    public string Controller { get; set; }

    /// <summary>
    /// 方法
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "方法")]
    public string Action { get; set; }

    /// <summary>
    /// 请求方式
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "请求方式")]
    public string Method { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "描述")]
    public string Description { get; set; }

    /// <summary>
    /// 请求url
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "请求URL")]
    public string RequestUrl { get; set; }

    /// <summary>
    /// 请求参数
    /// </summary>
    [SugarColumn(Length = 4000, IsNullable = true, ColumnDescription = "请求参数")]
    public string RequestParameters { get; set; }

    /// <summary>
    /// 异常短信息
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "异常短信息")]
    public string ExceptionMessage { get; set; }

    /// <summary>
    /// 异常完整信息
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "异常完整信息")]
    public string ExceptionMessageFull { get; set; }

    /// <summary>
    /// 异常堆栈信息
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "异常堆栈信息")]
    public string ExceptionStack { get; set; }

    /// <summary>
    /// 等级
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "异常类型")]
    public int LogLevel { get; set; }

    /// <summary>
    /// 请求ip
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "请求IP")]
    public string RequestIp { get; set; }

    /// <summary>
    /// ip所属真实地址
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "IP所属地址")]
    public string IpAddress { get; set; }

    /// <summary>
    /// 操作系统
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "操作系统")]
    public string OperatingSystem { get; set; }

    /// <summary>
    /// 设备类型
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "设备类型")]
    public string DeviceType { get; set; }

    /// <summary>
    /// 浏览器名称
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "浏览器名称")]
    public string BrowserName { get; set; }

    /// <summary>
    /// 浏览器版本
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "浏览器版本")]
    public string Version { get; set; }

    public bool IsDeleted { get; set; }
}
