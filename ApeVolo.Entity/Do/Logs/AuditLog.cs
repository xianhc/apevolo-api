using ApeVolo.Common.AttributeExt;
using SqlSugar;

namespace ApeVolo.Entity.Do.Logs;

[InitTable(typeof(AuditLog))]
[SugarTable("sys_audit_log", "审计日志表")]
public class AuditLog : BaseEntity
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
        ColumnDescription = "描述")]
    public string Description { get; set; }

    [SugarColumn(ColumnName = "request_url", ColumnDataType = "varchar", Length = 255, IsNullable = true,
        ColumnDescription = "请求URL")]
    public string RequestUrl { get; set; }

    [SugarColumn(ColumnName = "request_parameters", ColumnDataType = "longtext", IsNullable = true,
        ColumnDescription = "请求参数")]
    public string RequestParameters { get; set; }

    [SugarColumn(ColumnName = "response_data", ColumnDataType = "longtext", IsNullable = true,
        ColumnDescription = "响应数据")]
    public string ResponseData { get; set; }

    [SugarColumn(ColumnName = "execution_duration", IsNullable = true, ColumnDescription = "执行耗时(毫秒)")]
    public int ExecutionDuration { get; set; }


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