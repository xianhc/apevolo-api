using Ape.Volo.Common.Model;
using ApeVolo.Entity.Base;
using SqlSugar;

namespace ApeVolo.Entity.Monitor;

/// <summary>
/// 系统审计记录
/// </summary>
[SugarTable("log_audit")]
public class AuditLog : BaseEntity, ISoftDeletedEntity
{
    /// <summary>
    /// 区域
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public string Area { get; set; }

    /// <summary>
    /// 控制器
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public string Controller { get; set; }

    /// <summary>
    /// 方法
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public string Action { get; set; }

    /// <summary>
    /// 请求方式
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public string Method { get; set; }

    /// <summary>
    /// /描述
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public string Description { get; set; }

    /// <summary>
    /// 请求url
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public string RequestUrl { get; set; }

    /// <summary>
    /// 请求参数
    /// </summary>
    [SugarColumn(Length = 4000, IsNullable = true)]
    public string RequestParameters { get; set; }

    /// <summary>
    /// 响应数据
    /// </summary>
    [SugarColumn(Length = 4000, IsNullable = true)]
    public string ResponseData { get; set; }

    /// <summary>
    /// 执行耗时(毫秒)
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public int ExecutionDuration { get; set; }

    /// <summary>
    /// 请求IP
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public string RequestIp { get; set; }

    /// <summary>
    /// IP所属真实地址
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public string IpAddress { get; set; }


    /// <summary>
    /// 操作系统
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public string OperatingSystem { get; set; }

    /// <summary>
    /// 设备类型
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public string DeviceType { get; set; }

    /// <summary>
    /// 浏览器名称
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public string BrowserName { get; set; }

    /// <summary>
    /// 浏览器版本
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public string Version { get; set; }

    /// <summary>
    /// 是否已删除
    /// </summary>
    public bool IsDeleted { get; set; }
}
