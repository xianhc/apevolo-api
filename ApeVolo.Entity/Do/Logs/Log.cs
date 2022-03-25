using ApeVolo.Common.AttributeExt;
using SqlSugar;

namespace ApeVolo.Entity.Do.Logs;

[InitTable(typeof(Log))]
[SugarTable("sys_log", "系统日志表")]
public class Log : BaseEntity
{
    [SugarColumn(ColumnName = "area", ColumnDataType = "varchar", Length = 255, IsNullable = true,
        ColumnDescription = "区域")]
    public string Area { get; set; }

    [SugarColumn(ColumnName = "controller", ColumnDataType = "varchar", Length = 255, IsNullable = true,
        ColumnDescription = "控制器")]
    public string Controller { get; set; }

    [SugarColumn(ColumnName = "action", ColumnDataType = "varchar", Length = 255, IsNullable = true,
        ColumnDescription = "方法")]
    public string Action { get; set; }

    [SugarColumn(ColumnName = "method", ColumnDataType = "varchar", Length = 255, IsNullable = true,
        ColumnDescription = "请求方式")]
    public string Method { get; set; }

    [SugarColumn(ColumnName = "description", ColumnDataType = "varchar", Length = 255, IsNullable = true,
        ColumnDescription = "方法描述")]
    public string Description { get; set; }

    [SugarColumn(ColumnName = "request_url", ColumnDataType = "varchar", Length = 255, IsNullable = true,
        ColumnDescription = "请求URL")]
    public string RequestUrl { get; set; }

    [SugarColumn(ColumnName = "request_parameters", ColumnDataType = "longtext", IsNullable = true,
        ColumnDescription = "请求参数")]
    public string RequestParameters { get; set; }

    [SugarColumn(ColumnName = "exception_message", ColumnDataType = "longtext", IsNullable = true,
        ColumnDescription = "异常短信息")]
    public string ExceptionMessage { get; set; }

    [SugarColumn(ColumnName = "exception_message_full", ColumnDataType = "longtext", IsNullable = true,
        ColumnDescription = "异常完整信息")]
    public string ExceptionMessageFull { get; set; }

    [SugarColumn(ColumnName = "exception_stack", ColumnDataType = "longtext", IsNullable = true,
        ColumnDescription = "异常堆栈信息")]
    public string ExceptionStack { get; set; }

    [SugarColumn(ColumnName = "log_level", IsNullable = true, ColumnDescription = "异常类型")]
    public int LogLevel { get; set; }


    [SugarColumn(ColumnName = "browser_info", ColumnDataType = "varchar", Length = 255, IsNullable = true,
        ColumnDescription = "浏览器信息")]
    public string BrowserInfo { get; set; }

    [SugarColumn(ColumnName = "request_ip", ColumnDataType = "varchar", Length = 255, IsNullable = true,
        ColumnDescription = "请求IP")]
    public string RequestIp { get; set; }

    [SugarColumn(ColumnName = "ip_address", ColumnDataType = "varchar", Length = 255, IsNullable = true,
        ColumnDescription = "IP所属地址")]
    public string IpAddress { get; set; }
}